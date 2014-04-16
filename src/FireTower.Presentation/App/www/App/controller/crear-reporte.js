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

            $scope.createDisaster = function() {
                $scope.loading = $ionicLoading.show({
                    content: 'Guardando reporte...',
                    showBackdrop: false
                });

                var newDisaster = {
                    LocationDescription: $scope.LocationDescription,
                    Latitude: $scope.location.latitude,
                    Longitude: $scope.location.longitude,
                    ImageUri: $scope.imageUri
                };

                var action = disasterService.CreateDisaster(newDisaster);
                alert(action.then);
                alert(action.success);
                action
                    .then(function () {
                        alert("disaster created. Querying newest disasters...");
                        queryNewestDisasterUntilWeFindThisOne();
                    })
                    .catch(function (err) {
                        alert("Error creating disaster: " + JSON.stringify(err));
                        showMessage('Error', 'Error creando el reporte.');
                    })
                    .finally(function() {
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
                        viewModels.getMyLastReport(me.userId).success(function (lastReport) {
                            alert("Got latest: " + JSON.stringify(lastReport));
                            
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