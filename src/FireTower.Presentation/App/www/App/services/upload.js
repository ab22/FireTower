angular.module('firetower')
    .factory('PictureService', ['$q', function($q) {

        var uploadImage = function (targetUrl, fileURI, payload) {
            var def = $q.defer();
            var options = new FileUploadOptions();
            options.fileKey = "file";
            options.fileName = fileURI.substr(fileURI.lastIndexOf('/') + 1);
            options.mimeType = "image/jpeg";
            options.params = payload;

            var ft = new FileTransfer();
            ft.upload(fileURI, encodeURI(targetUrl), function (r) {
                def.resolve(r);
            }, function (e) {
                def.reject(e);
            }, options);
            return def.promise;
        };

        return {
            uploadImage: uploadImage
        };
    }]);

