angular.module('firetower')
    .factory('UploadService', ['$q', function($q) {

        var uploadImage = function (targetUrl, fileUri, payload, onProgress) {
            alert("Starting upload...");
            var def = $q.defer();
            var options = new FileUploadOptions();
            options.fileKey = "file";
            options.fileName = fileUri.substr(fileUri.lastIndexOf('/') + 1);
            options.mimeType = "image/jpeg";
            options.params = payload;
            
            var ft = new FileTransfer();
            ft.onprogress = onProgress;
            alert("Uploading to " + encodeURI(targetUrl));
            ft.upload(fileUri, encodeURI(targetUrl), function (r) {
                def.resolve(r);
            }, function (e) {
                alert("Error uploading file: " + JSON.stringify(e));
                def.reject(e);
            }, options);
            return def.promise;
        };

        return {
            uploadImage: uploadImage            
        };
    }]);

