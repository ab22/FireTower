angular.module('firetower', ['ionic', 'google-maps'])

    .run(function($ionicPlatform) {


        $ionicPlatform.ready(function() {

            if (window.StatusBar) {
                //StatusBar.hide();
                StatusBar.styleDefault();
            }
            
            if (!ionic.Platform.isCordova()) {
                
                function loadScript(url, callback) {
                    // Adding the script tag to the head as suggested before
                    var head = document.getElementsByTagName('head')[0];
                    var script = document.createElement('script');
                    script.type = 'text/javascript';
                    script.src = url;

                    // Then bind the event to the callback function.
                    // There are several events for cross browser compatibility.
                    script.onreadystatechange = callback;
                    script.onload = callback;

                    // Fire the loading
                    head.appendChild(script);
                }

                loadScript("app/js/cordova-2.0.0.js", function () {
                    alert("Loaded cordova-2.0.0.js");
                });

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
                'request': function(config) {
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
            .state('app.login',{
                url: "/login",
                templateUrl: "App/views/login.html",
                controller: "LoginController"
            })
            .state('otherwise', {
                url: '*path',
                templateUrl: 'App/views/splash.html',
                controller: 'SplashController'
            });
        OAuth.initialize('qZ4UVmAtk2MBWw1E5M4W1ru8QhA');


    }]);