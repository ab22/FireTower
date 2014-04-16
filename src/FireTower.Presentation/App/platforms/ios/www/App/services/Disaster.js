angular.module('firetower')
    .factory('DisasterService', ['$http','settings', function ($http, settings) {
        var factory = {};
        var token = localStorage.getItem('firetowertoken');
        
        factory.SaveSeverity = function (severity) {
            severity.token = token;
            return $http.post(settings.baseUrl + '/votes/severity', severity);
        };
        
        factory.VoteControlled = function (isControlled) {
            isControlled.token = token;
            return $http.post(settings.baseUrl + '/votes/controlled', isControlled);
        };
        
        factory.VotePutOut = function (putOutRequest) {
            putOutRequest.token = token;
            return $http.post(settings.baseUrl + '/votes/putout', putOutRequest);
        };

        factory.CreateDisaster = function (newDisaster) {
            newDisaster.token = token;
            alert(settings.baseUrl + '/Disasters' + " json: " + JSON.stringify(newDisaster));
            alert("Image size: " + newDisaster.FirstImageBase64.length);
            return $http.post(settings.baseUrl + '/Disasters', newDisaster);
        };

        factory.SaveImageToDisaster = function (disasterId, base64Image) {
            base64Image.token = token;
            throw new Error("Test");
            alert(settings.baseUrl + '/disasters/' + disasterId + '/image -- Image size: ' + base64Image.length);
            return $http.post(settings.baseUrl + '/disasters/' + disasterId + '/image', base64Image);
        };

        return factory;
}]);