angular.module('firetower')
    .controller('ReporteController', ['$location', '$scope', '$stateParams', '$ionicLoading', 'data', 'Math', 'DisasterService', '$ionicPopup', '$http', function ($location, $scope, $stateParams, $ionicLoading, data, Math, DisasterService, $ionicPopup, $http) {

        $scope.startCount = 5;
        var disasterId = -1;

        $scope.filedStars = [];
        $scope.blankStars = [];
        $scope.Severities = [1, 2, 3, 4, 5];

        var getArray = function(n, startingNumber) {
            var arr = [];
            for (var i = 0; i < n; i++)
                arr.push(startingNumber++);
            return arr;
        };

        $scope.saveSeverity = function(severityScore) {
            DisasterService.SaveSeverity({
                DisasterId: disasterId,
                Severity: severityScore
            })
                .success(function(response) {
                    $ionicPopup.alert({
                        title: 'Exito',
                        content: 'Su voto ha sido registrado.'
                    });
                })
                .error(function(error) {
                    alert(error);
                });
        };

        var pictureSource;
        var destinationType;

        ionic.Platform.ready(function() {
            if (!navigator.camera) {
                return;
            }
            pictureSource = navigator.camera.PictureSourceType.CAMERA;
            destinationType = navigator.camera.DestinationType.DATA_URL;
        });

        $scope.takePhoto = function() {

            var successCallback = function(base64foto) {

                $scope.loading = $ionicLoading.show({
                    content: 'Guardando foto...',
                    showBackdrop: false
                });

                DisasterService.SaveImageToDisaster(disasterId, { Base64Image: base64foto })
                    .success(function() {
                        $scope.reporte.Images.push("data:image/jpeg;base64," + base64foto);
                        $scope.loading.hide();
                    })
                    .error(function() {
                        $scope.loading.hide();
                        showMessage('Error', 'La foto no se pudo ser guardada.');
                    });
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

        var showMessage = function(title, message) {
            $ionicPopup.alert({
                title: title,
                content: message
            });
        };

        var init = function() {

            $scope.loading = $ionicLoading.show({
                content: 'Cargando datos del incendio...',
                showBackdrop: false
            });

            data.getReportById($stateParams.reporteId)
                .success(function(data) {
                    formatAndBindData(data[0]);
                })
                .error(function (error) {
                    $location.path('/app');
                    console.log(error);
                });

            $scope.marker = {
                coords: { latitude: 15.22, longitude: -89.88 }
            };

            $scope.map = {
                center: {
                    latitude: 0,
                    longitude: 1
                },
                zoom: 15,
                refresh: false
            };
        };

        var formatAndBindData = function(data) {
            disasterId = data.DisasterId;
            var formattedDate = data.CreatedDate.$date;
            formattedDate = moment((new Date()).toLocaleDateString()).fromNow();
            data.CreatedDate.$dateformatted = formattedDate;
            data.SeverityAverage = Math.Average(data.SeverityVotes);

            $scope.filedStars = getArray(data.SeverityAverage, 1);
            $scope.blankStars = getArray(5 - data.SeverityAverage, 1 + data.SeverityAverage);

            $scope.reporte = data;
            $scope.marker.coords = { latitude: data.Location[1], longitude: data.Location[0] };

            $scope.map = {
                center: $scope.marker.coords,
                zoom: 15,
                refresh: true
            };
            $scope.loading.hide();
        };

        $scope.isValidEmail = function(email) {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        };

        $scope.sendEmail = function(date, locationDescription, latitude, longitude) {
            $ionicPopup.prompt({
                title: 'Send Email',
                subTitle: 'Enter email to send disaster information',
                inputType: 'email',
                inputPlaceholder: 'Email Address'
            }).then(function(res) {
                if ($scope.isValidEmail(res)) {
                    if (res) {
                        $http.post('/SendDisasterByEmail', { Email: res, CreatedDate: date, LocationDescription: locationDescription, Latitude: latitude, Longitude: longitude }).success(function(response) {
                            $ionicPopup.alert({
                                title: 'Hey!',
                                content: 'Your message was successfully send.'
                            }).then(function(res) {

                            });
                        }).error(function(XMLHttpRequest, textStatus, errorThrown) {
                            $ionicPopup.alert({
                                title: 'Oh no!',
                                content: 'Your message could not be sent, try again later.'
                            }).then(function(res) {

                            });
                        });
                    } else {
                        $ionicPopup.alert({
                            title: 'Email error',
                            content: 'The email address you entered is invalid.'
                        }).then(function(res) {

                        });
                    }
                }
            });
        };

        $scope.reporte = null;
        $scope.isControlled = false;
        $scope.updateControlledFire = function() {
            if ($scope.isControlled)
                $scope.isControlled = false;
            else
                $scope.isControlled = true;

            DisasterService.VoteControlled({ DisasterId: disasterId, IsControlled: $scope.isControlled })
                .success(function(response) {
                    console.log(response);
                })
                .error(function(response) {
                    console.log(response);
                });

            console.log($scope.isControlled);

        };

        $scope.hasBeenPutOut = false;
        $scope.updatePutOutFire = function() {

            if ($scope.hasBeenPutOut)
                $scope.hasBeenPutOut = false;
            else
                $scope.hasBeenPutOut = true;

            DisasterService.VotePutOut({ DisasterId: disasterId, IsPutOut: $scope.hasBeenPutOut })
                .success(function(response) {
                    console.log(response);
                })
                .error(function(response) {
                    console.log(response);
                });

            console.log($scope.hasBeenPutOut);
        };
        init();
    }]);