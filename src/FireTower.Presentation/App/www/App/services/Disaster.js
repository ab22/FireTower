angular.module('firetower')
    .factory('DisasterService', ['UploadService', '$http', 'settings', function (upload, $http, settings) {
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
            var imageUri = newDisaster.ImageUri;
            delete newDisaster.ImageUri;
            newDisaster.token = token;
            alert(settings.baseUrl + '/Disasters' + " json: " + JSON.stringify(newDisaster));
            return upload.uploadImage(settings.baseUrl + '/Disasters', imageUri, newDisaster);
        };

        factory.SaveImageToDisaster = function (disasterId, imageUri) {            
            return upload.uploadImage(settings.baseUrl + '/disasters/' + disasterId + '/image', imageUri, { token: token });
        };

        return factory;
}]);