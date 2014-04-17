angular.module('firetower')
    .factory('cache', ['$q', function ($q) {
        return {
            set: function (key, data) {
                var d = $q.defer();
                window.localStorage.setItem(key, JSON.stringify(data));
                d.resolve();
                return d.promise;
            },
            get: function (key) {
                var d = $q.defer();
                var obj = JSON.parse(window.localStorage.getItem(key));
                d.resolve(obj);
                return d.promise;
            }
        };
    }]);