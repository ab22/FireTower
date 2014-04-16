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
            takePicture: function() {
                var def = $q.defer();

                var options = {
                    quality: 50,
                    destinationType: 1, //FILE_URI
                    sourceType: 0, //PHOTOLIBRARY
                    encodingType: 0, //JPEG
                    MediaType: 0 //IMAGE
                };

                navigator.camera.getPicture(
                    function (imageUri) {
                        alert("got photo: " + imageUri);
                        def.resolve(imageUri);
                    },
                    function(err) {
                        alert("There was an error when taking a photo with the device's camera. " + err);
                        def.reject(err);
                    },
                    options);

                return def.promise;
            }
        };
    }]);