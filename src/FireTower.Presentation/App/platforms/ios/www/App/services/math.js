angular.module('firetower')
    .factory('Math', ['$http', function ($http) {
        var factory = {};

        factory.Average = function (array) {
            var result = 0;

            for (var i = 0; i < array.length; i++) {
                result += array[i];
            }

            return result / array.length;
        };

        return factory;
}]);