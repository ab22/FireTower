/// <reference path="login.js" />
angular.module('firetower').controller('LoginController', ['$scope', '$timeout', '$location', 'userManagement', 'loginService', '$ionicLoading', function ($scope, $timeout, $location, user, loginService, $ionicLoading) {
    $scope.logged = false;
    $scope.salutation = false;
    $scope.byebye = false;
    $scope.facebookReady = false;
    
    //if the user is logged in, send them to home
    if (localStorage.getItem("firetowertoken"))
        $location.path("/app/reportes");
    
    $scope.login = function () {
        OAuth.popup('facebook', function (err, result) {
            result.get('/me').done(function (data) {
                user.setUser(data);
            });
        });
    };

    // BASIC LOGIN
    $scope.data = { };
    $scope.basicLogin = function () {
        $scope.loading = $ionicLoading.show({
            content: 'Iniciando Sesion...',
            showBackdrop: false
        });
        var email = $scope.data.email;
        var password = $scope.data.password;
        loginService.authenticate(email, password).success(function (response) {
            var token = response.token;
            localStorage.setItem("firetowertoken", token);
            $scope.loading.hide();
            $location.path("/app/reportes");
        }).error(function (error) {
            $scope.loading.hide();
            $location.path("/error");
        });
    };
}]);