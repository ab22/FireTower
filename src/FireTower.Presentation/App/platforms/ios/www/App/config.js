angular.module('firetower', ['ionic', 'google-maps'])
    .run(function($ionicPlatform) {
        $ionicPlatform.ready(function() {
            if (window.StatusBar) {
                //StatusBar.styleDefault();
                StatusBar.hide();
            }
        });
    })
    .config(['$httpProvider', function($httpProvider) {
        $httpProvider.interceptors.push(['$q', '$location', function($q, $location) {
            return {
                'responseError': function(rejection) {
                    if (rejection.status == 401 || rejection.status == 403) {
                        var token = 'firetowertoken';
                        if (token in localStorage) localStorage.removeItem(token);
                        $location.path('/app/');
                    }
                    $q.reject(rejection);
                }
            };
        }]);
    }])
    .config(['$httpProvider', function($httpProvider) {
        $httpProvider.interceptors.push(['$q', '$location', function($q, $location) {
            return {
                'request': function (config) {
                    console.log(config.method + " " + config.url);
                    return config || $q.when(config);
                }
            };
        }]);
    }])
    .config(['$stateProvider', function($stateProvider) {
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
            .state('app.crearReporte', {
                url: "/crear-reporte",
                views: {
                    'menuContent': {
                        templateUrl: "App/views/crear-reporte.html",
                        controller: 'NewReportController'
                    }
                }
            })
            .state('otherwise', {
                url: '*path',
                templateUrl: 'App/views/login.html',
                controller: 'LoginController'
            });
        OAuth.initialize('qZ4UVmAtk2MBWw1E5M4W1ru8QhA');


    }]);