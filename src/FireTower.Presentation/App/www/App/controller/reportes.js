angular.module('firetower')
    .controller('ReportesController', ['LocationService', 'cache', '$scope', '$ionicLoading', 'data', 'Math', '$ionicPopup', '$http', '$location',
        function(locationService, cache, $scope, $ionicLoading, data, Math, $ionicPopup, $http, $location) {

            $scope.viewReport = function(disasterId) {
                $location.path("/app/reporte/" + disasterId);
            };

            $scope.refreshReports = function() {
                getAllReports();
                $scope.$broadcast('scroll.refreshComplete');
                $scope.$broadcast('scroll.resize');
            };

            var setIncendios = function(data) {
                for (var i = 0; i < data.length; i++) {
                    var formattedDate = moment(data[i].CreatedDate.$date).fromNow();
                    data[i].CreatedDate.$dateformatted = formattedDate;
                    data[i].SeverityAverage = Math.Average(data[i].SeverityVotes);
                }

                $scope.reportes = data;
            };

            var getAllReports = function() {
                $scope.loadingFires = $ionicLoading.show({
                    content: 'Cargando datos de incendios...',
                    animation: 'fade-in',
                    showBackdrop: false,
                    maxWidth: 200,
                    showDelay: 500
                });

                cache.get("reports").then(function(cachedData) {
                    if (cachedData) {
                        setIncendios(cachedData);
                        $scope.loadingFires.hide();
                    }
                });

                locationService.getCurrentPosition()
                    .then(function(locationResponse) {
                        data.getAllReports(locationResponse)
                            .success(function(dataFromServer) {
                                cache.set("reports", dataFromServer);
                                setIncendios(dataFromServer);
                                $scope.loadingFires.hide();
                            })
                            .error(function() {
                                $scope.loadingFires.hide();
                                showMessage('Error', 'No hemos podido cargar los reportes. Estas conectado a internet?');
                            });
                    }).catch(function () {
                        alert("Your location could not be determined. Data could not be loaded.");
                    });

            };

            var showMessage = function(title, message) {
                $ionicPopup.alert({
                    title: title,
                    content: message
                });
            };

            $scope.isValidEmail = function(email) {
                var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                return re.test(email);
            };

            $scope.reportes = null;
            getAllReports();

            $scope.crearReporte = function() {
                $location.path('/app/crear-reporte');
            };
        }]);