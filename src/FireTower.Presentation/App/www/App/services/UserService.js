angular.module('firetower')
    .factory('UserService', ['$http', function ($http) {
    var factory = {};
    var baseUrl = 'http://firetowerapidev.apphb.com/';
    factory.getUser = function () {
        var token = localStorage.getItem('firetowertoken');       
        return $http.get(baseUrl + '/me?token=' + token);
    };

    return factory;
}]);