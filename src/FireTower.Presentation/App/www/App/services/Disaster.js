angular.module('firetower')
    .factory('DisasterService', ['$http', function ($http) {
        var factory = {};
        var token = localStorage.getItem('firetowertoken');
        var baseUrl = 'http://firetowerapidev.apphb.com';

        factory.SaveSeverity = function (severity) {
            severity.token = token;
            return $http.post(baseUrl + '/votes/severity', severity);
        };
        
        factory.VoteControlled = function (isControlled) {
            isControlled.token = token;
            return $http.post(baseUrl + '/votes/controlled', isControlled);
        };
        
        factory.VotePutOut = function (putOutRequest) {
            putOutRequest.token = token;
            return $http.post(baseUrl + '/votes/putout', putOutRequest);
        };

        factory.CreateDisaster = function (newDisaster) {
            newDisaster.token = token;
            return $http.post(baseUrl + '/Disasters', newDisaster);
        };

        factory.SaveImageToDisaster = function (disasterId, base64Image) {
            base64Image.token = token;
            return $http.post(baseUrl + '/disasters/' + disasterId + '/image', base64Image);
        };

        return factory;
}]);