angular.module('firetower')
    .controller('NewReportController', ['$location', 'data', '$scope', '$ionicPopup', 'DisasterService', 'PictureService', 'LocationService', '$ionicLoading',
        function($location, viewModels, $scope, $ionicPopup, disasterService, pictureService, locationService, $ionicLoading) {

            var setDisasterPosition = function(lat, lng) {
                $scope.location = { latitude: lat, longitude: lng };
                locationService.getLocationAddress(lat, lng)
                    .then(function(locationData) {
                        $scope.LocationDescription = locationData.address;
                    });
            };

            var initializeMap = function() {
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
                        dragend: function(marker) {
                            this.coords.latitude = marker.getPosition().lat();
                            this.coords.longitude = marker.getPosition().lng();
                            setDisasterPosition(marker.getPosition().lat(), marker.getPosition().lng());
                        }
                    }
                };
            };

            var init = function() {

                initializeMap();

                pictureService.takePicture().then(function(imageUri) {
                    $scope.imageUri = imageUri;
                }).finally(function() {

                    locationService.getCurrentPosition()
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
                pictureService.init();
                init();
            });

            function makeid() {
                var text = "";
                var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                for (var i = 0; i < 5; i++)
                    text += possible.charAt(Math.floor(Math.random() * possible.length));

                return text;
            }

            var fetchToken;

            $scope.createDisaster = function() {
                $scope.uploadIndicator = $ionicLoading.show({
                    content: 'Guardando reporte...',
                    animation: 'fade-in',
                    showBackdrop: false,
                    maxWidth: 200,
                    showDelay: 500
                });

                fetchToken = makeid();

                var newDisaster = {
                    LocationDescription: $scope.LocationDescription,
                    Latitude: $scope.location.latitude,
                    Longitude: $scope.location.longitude,
                    ImageUri: $scope.imageUri,
                    FetchToken: fetchToken
                };

                disasterService.CreateDisaster(newDisaster)
                    .then(function () {
                        $scope.uploadIndicator.hide();
                        queryNewestDisasterUntilWeFindThisOne();                        
                    })
                    .catch(function(err) {
                        showMessage('Error', 'Error creando el reporte.');
                        $scope.uploadIndicator.hide();
                    });
            };

            var queryNewestDisasterUntilWeFindThisOne = function() {

                $scope.terminandoIndicator = $ionicLoading.show({
                    content: 'Terminando reporte...',
                    animation: 'fade-in',
                    showBackdrop: false,
                    maxWidth: 200,
                    showDelay: 500
                });

                var interval = setInterval(function() {
                    viewModels.getDisasterByFetchToken(fetchToken).success(function(lastReport) {
                        if (lastReport.length == 0) return;

                        clearInterval(interval);
                        $scope.terminandoIndicator.hide();
                        showDetails(lastReport[0].DisasterId);
                    });
                }, 1000);
            };

            var showDetails = function(disasterId) {
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