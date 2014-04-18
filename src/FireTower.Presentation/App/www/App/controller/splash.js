angular
    .module('firetower')
    .controller('SplashController',
        ['$scope', '$timeout', '$location',
            function ($scope, $timeout, $location) {


                if (localStorage.getItem("firetowertoken")){
                    $location.path("/app/reportes");
                }
                else{                    
                    $location.path("/login");
                }


}]);