﻿angular.module('firetower')
    .controller('NewReportController', ['$location', 'data', '$scope', '$ionicPopup', 'DisasterService', 'PictureService', 'LocationService', '$ionicLoading', 'UserService', function($location, viewModels, $scope, $ionicPopup, DisasterService, PictureService, LocationService, $ionicLoading, UserService) {

        var setDisasterPosition = function(lat, lng) {
            $scope.location = { latitude: lat, longitude: lng };
            LocationService.getLocationAddress(lat, lng)
                .then(function(locationData) {
                    $scope.LocationDescription = locationData.address;                    
                });            
        };

        var initializeMap = function () {
            $scope.location = { latitude: 0, longitude: 0 };

            $scope.map = {
                center: $scope.location,
                zoom: 1,
                maptype: "satellite"
            };

            $scope.marker = {
                coords: { latitude: 0, longitude: 0 },
                options: { draggable: true },
                events: {
                    dragend: function (marker) {
                        this.coords.latitude = marker.getPosition().lat();
                        this.coords.longitude = marker.getPosition().lng();
                        setDisasterPosition(marker.getPosition().lat(), marker.getPosition().lng());
                    }
                }
            };
        };
        
        var init = function() {

            initializeMap();
            
            PictureService.takePicture().then(function (data) {
                $scope.base64foto = data.base64;
                $scope.foto = data.imageUrl;
            }).finally(function() {

                LocationService.getCurrentPosition()
                    .catch(function(err) {
                        alert("Lo sentimos, pero no se puede crear un reporte sin ubicacion.");
                        $location.path('/app');
                    })
                    .then(function(locationData) {
                        
                        setDisasterPosition(locationData.lat, locationData.lng);

                        $scope.map = {
                            center: $scope.location,
                            zoom: 15,
                            maptype: "satellite"
                        };

                        $scope.marker.coords = {
                            latitude: $scope.location.latitude,
                            longitude: $scope.location.longitude
                        };                        
                    });
            });
        };

        $scope.data = { };
        $scope.obj = { };

        ionic.Platform.ready(function() {
            PictureService.init();
            init();
        });

        $scope.createDisaster = function() {
            $scope.loading = $ionicLoading.show({
                content: 'Guardando reporte...',
                showBackdrop: false
            });

            var action = DisasterService.CreateDisaster({
                LocationDescription: $scope.LocationDescription,
                Latitude: $scope.location.latitude,
                Longitude: $scope.location.longitude,
                FirstImageBase64: $scope.base64foto
            });

            action
                .success(function() {
                    queryNewestDisasterUntilWeFindThisOne();
                })
                .error(function () {
                    showMessage('Error', 'Error creando el reporte.');
                })
                .finally(function () {
                    $scope.loading.hide();
                });
        };

        var queryNewestDisasterUntilWeFindThisOne = function() {
            $scope.loading = $ionicLoading.show({
                content: 'Terminando reporte...',
                showBackdrop: false
            });

            viewModels.getUser().success(function(me) {
                var interval = setInterval(function() {
                    viewModels.getMyLastReport(me.userId).success(function(lastReport) {
                        if (lastReport.length == 0) return;

                        if (moment(lastReport[0].CreatedDate.$date).add('seconds', 10) > moment()) {
                            clearInterval(interval);
                            showDetails(lastReport[0].DisasterId);
                        }
                    });
                }, 1000);
            });
        };

        var showDetails = function(disasterId) {
            $scope.loading.hide();
            showMessage('Exito!', 'Reporte creado exitosamente!');
            $location.path('/app/reporte/' + disasterId);
        };

        var showMessage = function(title, message) {
            $ionicPopup.alert({
                title: title,
                content: message
            });
        };
    }]);