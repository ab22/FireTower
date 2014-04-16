angular.module('firetower').controller('MenuController', ['$scope', '$location', 'userManagement', function ($scope, $location, user) {
    
    $scope.logout = function () {
       user.logoutUser();
    };
    
}]);