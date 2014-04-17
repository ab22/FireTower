angular.module('firetower')
    .factory('PictureService', ['$q', function($q) {


        return {
            init: function() {
                if (!navigator.camera) {
                    navigator.camera = {
                        getPicture: function(successCallback) {
                            alert("Simulating the camera. Will return a default image.");
                            successCallback("http://www.wildlandfire.com/pics/wall/wildfire_elkbath.jpg");
                        }
                    };
                }
            },
            takePicture: function(source) {
                var def = $q.defer();

                var sourceType = 0; //library
                if (source == "camera")
                    sourceType = 1;
                
                var options = {
                    quality: 50,
                    destinationType: 1, //FILE_URI
                    sourceType: sourceType,
                    encodingType: 0, //JPEG
                    MediaType: 0 //IMAGE
                };

                navigator.camera.getPicture(
                    function (imageUri) {                        
                        def.resolve(imageUri);
                    },
                    function(err) {
                        def.reject(err);
                    },
                    options);

                return def.promise;
            }
        };
    }]);