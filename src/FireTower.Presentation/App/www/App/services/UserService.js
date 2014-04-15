angular.module('firetower')
    .factory('UserService', ['$http', 'settings', function($http, settings) {
        var factory = { };
        factory.getUser = function() {
            var token = localStorage.getItem('firetowertoken');
            return $http.get(settings.baseUrl + '/me?token=' + token);
        };

        return factory;
    }]);