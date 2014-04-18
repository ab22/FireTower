/// <reference path="login.js" />
angular.module('firetower').controller('LoginController',
    ['$scope', '$timeout', '$location', 'userManagement', 'loginService', '$ionicLoading',
        function($scope, $timeout, $location, user, loginService, $ionicLoading) {            
            $scope.logged = false;
            $scope.salutation = false;
            $scope.byebye = false;
            $scope.facebookReady = false;

            if (localStorage.getItem("firetowertoken"))
                $location.path("/app/reportes");

            $scope.data = {};

            $scope.facebookLogin = function() {
                OAuth.popup('facebook', function(err, result) {
                    result.get('/me').done(function(data) {
                        user.setUser(data);
                    });
                });
            };

            $scope.basicLogin = function () {
                $scope.loginLoading = $ionicLoading.show({
                    content: 'Iniciando Sesion...',
                    animation: 'fade-in',
                    showBackdrop: false,
                    maxWidth: 200,
                    showDelay: 500
                });

                var email = $scope.data.email;
                var password = $scope.data.password;
                loginService.authenticate(email, password).success(function(response) {
                    var token = response.token;
                    localStorage.setItem("firetowertoken", token);
                    $scope.loginLoading.hide();
                    $location.path("/app/reportes");
                }).error(function(error) {
                    $scope.loginLoading.hide();
                    $location.path("/error");
                });
            };
        }]);