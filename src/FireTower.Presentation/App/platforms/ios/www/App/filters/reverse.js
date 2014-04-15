angular.module('firetower').filter('reverse', function () {
    return function (items) {
        return items == null ? null : items.slice().reverse();
    };
});