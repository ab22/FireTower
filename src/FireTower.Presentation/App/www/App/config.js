angular.module('firetower', ['ionic', 'google-maps'])

.run(function ($ionicPlatform) {
    $ionicPlatform.ready(function () {
        if (window.StatusBar) {
            StatusBar.styleDefault();
        }
    });
})

.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $stateProvider
        .state('app', {
            url: "/app",
            templateUrl: "App/views/menu.html",
            controller: "MenuController"
        })
        .state('app.inicio', {
            url: "/inicio",
            views: {
                'menuContent': {
                    templateUrl: "App/views/inicio.html"
                }
            }
        })
        .state('app.reportes', {
            url: "/reportes",
            views: {
                'menuContent': {
                    templateUrl: "App/views/reportes.html",
                    controller: 'ReportesController'
                }
            }
        })
        .state('app.reporte', {
            url: "/reporte/:reporteId",
            views: {
                'menuContent': {
                    templateUrl: "App/views/reporte.html",
                    controller: 'ReporteController'
                }
            }
        })
        .state('app.newfirelocation', {
            url: "/newfirelocation",
            views: {
                'menuContent': {
                    templateUrl: "App/views/new-fire-location.html",
                    controller: 'NewFireLocationController'
                }
            }
        })
        .state('otherwise', {
            url: '*path',
            templateUrl: 'App/views/reportes.html',
            controller: 'ReportesController'
        });
    OAuth.initialize('qZ4UVmAtk2MBWw1E5M4W1ru8QhA');
}]);
