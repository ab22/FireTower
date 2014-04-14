angular.module('firetower')
    .controller('NewReportController', ['data', '$scope', '$ionicPopup', 'DisasterService', 'PictureService', '$location', '$ionicLoading', 'UserService', function(viewModels, $scope, $ionicPopup, DisasterService, PictureService, $location, $ionicLoading, UserService) {

        var modelId = null;

        var init = function () {
            $scope.takePicture();

            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(getCurrentPosition, positionFailure);
            }

            $scope.base64foto = PictureService.getDefaultPictureWithoutDataType();
        };

        $scope.data = { };
        $scope.obj = { };
        var pictureSource;
        var destinationType;
        
        ionic.Platform.ready(function() {
            if (!navigator.camera) {
                return;
            }
            pictureSource = navigator.camera.PictureSourceType.CAMERA;
            destinationType = navigator.camera.DestinationType.DATA_URL;
        });

        $scope.takePicture = function () {

            var successCallback = function(base64foto) {
                $scope.base64foto = base64foto;
                $scope.foto = "data:image/jpeg;base64," + base64foto;
            };

            var options = {
                quality: 50,
                destinationType: destinationType,
                sourceType: pictureSource,
                encodingType: 0
            };
            if (!navigator.camera) {
                var photo = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAAsTAAALEwEAmpwYAAAABGdBTUEAALGOfPtRkwAAACBjSFJNAAB6JQAAgIMAAPn/AACA6QAAdTAAAOpgAAA6mAAAF2+SX8VGAAABLElEQVR42qSTQStFURSFP7f3XygyoAwoYSYMPCIpk2egMFSmUvwCRpSRDIwYGbwyVuYykB9y914m951z7nHe6J26dc9u77XXWmdvJLF7/audqx9JYuvyW92LL0li8K2df2r17CPEVk7ftXTclyQqAMmRCwC5I3fS42a4W7y74VYDNAAuJA8AaXIsSACsDgAdAJeFrnnyoMBygKZJJ3b1It0AmsTMDPdEgrujJqHEwCxqznMaD2KgyCDRnEuo8qJhHvx/hcQDbzGoix5Yi4G1TcwZWNEDKwJU+WDkhg2ToDaD+M65YcVB8jg3Y5IY5VQAyyf9gLJw+CqAuYNnAczsPQpgevtBU937kDexcdssj8Ti0ZskMd97CRs3u//U2sjJzbtwH1+/Cf8jS/gbAMmWc42HzdIjAAAAAElFTkSuQmCC";
                successCallback(photo);
                return;
            }
            navigator.camera.getPicture(
                successCallback,
                function(err) {
                },
                options);
        };

        $scope.createDisaster = function() {
            $scope.loading = $ionicLoading.show({
                content: 'Guardando reporte...',
                showBackdrop: false
            });

            DisasterService.CreateDisaster({
                LocationDescription: $scope.LocationDescription,
                Latitude: $scope.location.latitude,
                Longitude: $scope.location.longitude,
                FirstImageBase64: $scope.base64foto
            }).success(function() {
                //loop querying the newest disasters until we have ours

                viewModels.getUser().success(function (me) {
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
            }).error(function() {
                showMessage('Error', 'Error creando el reporte.');                
            });
        };

        var showDetails = function(disasterId) {
            $scope.loading.hide();
            showMessage('Exito!', 'Reporte creado exitosamente!');
            $location.path('/app/reporte/' + disasterId);
        };

        var getLocationAddress = function(latLng) {
            var geocoder = new google.maps.Geocoder();

            geocoder.geocode({ 'latLng': latLng }, function(results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    if (results[1]) {
                        $scope.LocationDescription = results[1].formatted_address;
                    } else {
                        alert('No results found');
                    }
                } else {
                    alert('Geocoder failed due to: ' + status);
                }
            });
        };

        var getCurrentPosition = function(position) {
            $scope.location = {
                latitude: position.coords.latitude,
                longitude: position.coords.longitude
            };

            $scope.marker.coords = {
                latitude: $scope.location.latitude,
                longitude: $scope.location.longitude
            };

            $scope.map = {
                center: $scope.location,
                zoom: 15
            };

            var lat = $scope.location.latitude;
            var lng = $scope.location.longitude;
            var latlng = new google.maps.LatLng(lat, lng);

            getLocationAddress(latlng);
        };

        var positionFailure = function(error) {
            console.log(error.message);
        };

        $scope.location = { latitude: 15.22, longitude: -89.88 };
        $scope.marker = {
            coords: { latitude: 15.22, longitude: -89.88 },
            options: { draggable: true },
            events: {
                dragend: function(marker) {
                    this.coords.latitude = marker.getPosition().lat();
                    this.coords.longitude = marker.getPosition().lng();

                    getLocationAddress(marker.getPosition());
                }
            }
        };

        $scope.map = {
            center: $scope.location,
            zoom: 15
        };

        var showMessage = function(title, message) {
            $ionicPopup.alert({
                title: title,
                content: message
            });
        };

        init();
    }]);