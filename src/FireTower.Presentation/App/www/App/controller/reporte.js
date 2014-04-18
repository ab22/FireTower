angular.module('firetower')
    .controller('ReporteController', ['PictureService', '$location', '$scope', '$stateParams', '$ionicLoading', 'data', 'Math', 'DisasterService', '$ionicPopup', '$http',
        function(pictureService, $location, $scope, $stateParams, $ionicLoading, data, math, disasterService, $ionicPopup, $http) {

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

                $scope.selectedSeverity = severityScore;

                disasterService.SaveSeverity({
                    DisasterId: disasterId,
                    Severity: severityScore
                });                    
            };


            ionic.Platform.ready(function() {
                pictureService.init();
            });

            $scope.takePhoto = function() {

                $scope.imageUploadingMessage = 'Guardando foto... ';
                pictureService.takePicture()
                    .then(function(imageUri) {
                        $scope.uploadIndicator = $ionicLoading.show({
                            content: $scope.imageUploadingMessage,
                            animation: 'fade-in',
                            showBackdrop: false,
                            maxWidth: 200,
                            showDelay: 500
                        });

                        var progress = function(e) {
                            var complete = 0;
                            if (e.lengthComputable) {
                                complete = e.loaded / e.total;
                            } else {
                                complete++;
                            }
                            //$scope.uploadIndicator.content = 'Guardando foto... ' + complete + "%";
                        };

                        disasterService.SaveImageToDisaster(disasterId, imageUri, progress).then(function() {
                            $scope.reporte.Images.push(imageUri);
                            $scope.uploadIndicator.hide();
                        }).catch(function() {
                            $scope.uploadIndicator.hide();
                            showMessage('Error', 'La foto no se pudo ser guardada.');
                        });

                    });
            };

            var showMessage = function(title, message) {
                $ionicPopup.alert({
                    title: title,
                    content: message
                });
            };

            var init = function() {

                $scope.loadingFire = $ionicLoading.show({
                    content: 'Cargando datos del incendio...',
                    animation: 'fade-in',
                    showBackdrop: false,
                    maxWidth: 200,
                    showDelay: 500
                });

                data.getReportById($stateParams.reporteId)
                    .success(function(data) {
                        formatAndBindData(data[0]);
                    })
                    .error(function(error) {
                        $location.path('/app');
                        console.log(error);
                    });

                $scope.marker = {
                    coords: { latitude: 15.22, longitude: -89.88 }
                };

                $scope.map = {
                    center: { latitude: 0, longitude: 0},
                    zoom: 15,
                    refresh: false,
                    options: {
                        draggable: false,
                        minZoom: 15,
                        disableDefaultUI: true,
                        mapTypeId: google.maps.MapTypeId.HYBRID
                    }                    
                };
            };

            var formatAndBindData = function(data) {
                disasterId = data.DisasterId;
                var formattedDate = data.CreatedDate.$date;
                formattedDate = moment((new Date()).toLocaleDateString()).fromNow();
                data.CreatedDate.$dateformatted = formattedDate;
                data.SeverityAverage = math.Average(data.SeverityVotes);

                $scope.filedStars = getArray(data.SeverityAverage, 1);
                $scope.blankStars = getArray(5 - data.SeverityAverage, 1 + data.SeverityAverage);

                $scope.reporte = data;
                $scope.marker.coords = { latitude: data.Location[1], longitude: data.Location[0] };
                $scope.coordsFormat = "http://maps.google.com/maps?q="+$scope.marker.coords.latitude+","+$scope.marker.coords.longitude;

                $scope.map = {
                    center: $scope.marker.coords,
                    zoom: 15,
                    refresh: true
                };
                $scope.loadingFire.hide();
            };

            $scope.loadMaps = function(){
                window.location = $scope.coordsFormat;
            }

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

                disasterService.VoteControlled({ DisasterId: disasterId, IsControlled: $scope.isControlled })
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

                disasterService.VotePutOut({ DisasterId: disasterId, IsPutOut: $scope.hasBeenPutOut })
                    .success(function(response) {
                        console.log(response);
                    })
                    .error(function(response) {
                        console.log(response);
                    });

                console.log($scope.hasBeenPutOut);
            };

            $scope.backToReportes = function() {
                $location.path('/reportes');
            };
            init();
        }]);