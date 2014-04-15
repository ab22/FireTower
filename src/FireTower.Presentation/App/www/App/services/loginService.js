angular.module('firetower')
    .service('loginService', ['$q', '$http','settings', function ($q, $http, settings) {
        
        return {
            authenticate: function (email, password) {
                return $http.post(settings.baseUrl + "/login", { email: email, password: password });
            }
        };
    }]);