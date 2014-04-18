angular.module('firetower')
    .factory('data', ['$http', 'settings', function($http, settings) {
        var factory = { };

        var apiKey = 'Edc1w2R7eTgTWs5fUOrbiI8-xkDkPznM';
        var baseUrl = 'https://api.mongolab.com/api/1/databases/';
        var db = 'appharbor_ab50c767-930d-4b7d-9571-dd2a0b62d5a9';
        var collection = 'DisasterViewModel';

        factory.getAllReports = function(location) {
            
            var query = {                
                "Location":
                    {
                        $near:
                            {
                                $geometry: { type: "Point", coordinates: [location.lng, location.lat] },
                                $maxDistance: 10000
                            }
                    }
            };
            
            var url = baseUrl + db + '/collections/' + collection + '?apiKey=' + apiKey + '&q=' + encodeURIComponent(JSON.stringify(query));
            return $http.get(url).error(function(err) {
                alert("http error: " + err);
            }).success(function (d) {
                console.log("got view model data: " + JSON.stringify(d));
            });
        };

        factory.getDisasterByFetchToken = function(fetchToken) {
            var url = baseUrl + db + '/collections/' + collection + '?apiKey=' + apiKey + '&q={"FetchToken":"' + fetchToken + '"}';
            return $http.get(url);
        };

        factory.getReportById = function(id) {
            var query = '?q={"DisasterId": "' + id + '"}';
            return $http.get(baseUrl + db + '/collections/' + collection + query + '&apiKey=' + apiKey);
        };

        factory.getUser = function() {
            var token = localStorage.getItem('firetowertoken');
            return $http.get(settings.baseUrl + '/me?token=' + token);
        };

        return factory;
    }]);
