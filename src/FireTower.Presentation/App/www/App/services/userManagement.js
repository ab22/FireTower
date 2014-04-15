angular.module('firetower').factory('userManagement', ['$http', '$location', 'settings', function userManagementFactory($http, $location, settings) {
    var user;

    var clearUser = function() {
        var token = 'firetowertoken';
        if (token in localStorage) localStorage.removeItem(token);
        $location.path('/app/');
    };
    
    return {
        setUser: function(facebookUser) {
            var loginSuccess = function(responselogin) {
                user = facebookUser;
                var token = responselogin.token;
                localStorage.setItem("firetowertoken", token);
                $location.path('/app/reportes');
            };

            $http.post(settings.baseUrl + '/login/facebook', { FacebookId: parseInt(facebookUser.id) })
                .success(loginSuccess)
                .error(function(xhr, statusCode) {
                    if (statusCode == "401") {
                        $http.post(settings.baseUrl + '/user/facebook', {
                            FirstName: facebookUser.first_name,
                            LastName: facebookUser.last_name,
                            Name: facebookUser.name,
                            FacebookId: facebookUser.id,
                            Locale: facebookUser.locale,
                            Username: facebookUser.username,
                            Verified: facebookUser.verified
                        }).success(function() {
                            $http.post(settings.baseUrl + '/login/facebook', { FacebookId: parseInt(facebookUser.id) })
                                .success(loginSuccess);
                        });
                    }
                });

        },
        getUser: function() {
            return user;
        },
        logoutUser: function () {            
            if (user != undefined) {
                $http.post(settings.baseUrl + '/logout', { FacebookId: parseInt(user.id) });
            }
            clearUser();
        },
        clearUser: function () {
            clearUser();
        }
    };
}]);