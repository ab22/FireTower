angular.module('firetower')
    .controller('ReportesController', ['$scope', '$ionicLoading', 'data', 'Math', '$ionicPopup', '$http', '$location', function($scope, $ionicLoading, data, Math, $ionicPopup, $http, $location) {

        $scope.viewReport = function(disasterId) {
            $location.path("/app/reporte/" + disasterId);
        };
        
        var getAllReports = function () {
            $scope.loadingFires = $ionicLoading.show({
                content: 'Cargando datos de incendios...',
                animation: 'fade-in',
                showBackdrop: false,
                maxWidth: 200,
                showDelay: 500
            });

            data.getAllReports()
                .success(function(data) {
                    $scope.reportes = data;
                    for (var i = 0; i < $scope.reportes.length; i++) {
                        var formattedDate = $scope.reportes[i].CreatedDate.$date;
                        formattedDate = moment((new Date()).toLocaleDateString()).fromNow();
                        $scope.reportes[i].CreatedDate.$dateformatted = formattedDate;
                        $scope.reportes[i].SeverityAverage = Math.Average($scope.reportes[i].SeverityVotes);
                    }

                    $scope.loadingFires.hide();
                })
                .error(function() {
                    $scope.loadingFires.hide();
                    showMessage('Error', 'No hemos podido cargar los reportes. Estas conectado a internet?');
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