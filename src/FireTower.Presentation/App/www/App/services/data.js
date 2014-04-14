angular.module('firetower')
    .factory('data', ['$http', function($http) {
        var factory = {};

        var apiKey = 'Edc1w2R7eTgTWs5fUOrbiI8-xkDkPznM';
        var baseUrl = 'https://api.mongolab.com/api/1/databases/';
        var db = 'appharbor_ab50c767-930d-4b7d-9571-dd2a0b62d5a9';
        var collection = 'DisasterViewModel';

        factory.getAllReports = function () {
            var url = baseUrl + db + '/collections/' + collection + '?apiKey=' + apiKey;
            return $http.get(url);
        };

        factory.getMyLastReport = function (userId) {
            var url = baseUrl + db + '/collections/' + collection + '?apiKey=' + apiKey + '&q={"UserId":"'+userId+'"}&l=1&s={"CreatedDate":-1}';            
            return $http.get(url);
        };

        factory.getReportById = function (id) {
            var query = '?q={"DisasterId": "' + id + '"}';
            return $http.get(baseUrl + db + '/collections/' + collection + query + '&apiKey=' + apiKey);
        };

        factory.getUser = function() {
            var token = localStorage.getItem('firetowertoken');
            return $http.get('/me?token=' + token);
        };

        return factory;
    }]);