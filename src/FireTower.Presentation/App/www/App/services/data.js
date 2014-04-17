﻿angular.module('firetower')
    .factory('data', ['$http', 'settings', function($http, settings) {
        var factory = { };

        var apiKey = 'Edc1w2R7eTgTWs5fUOrbiI8-xkDkPznM';
        var baseUrl = 'https://api.mongolab.com/api/1/databases/';
        var db = 'appharbor_ab50c767-930d-4b7d-9571-dd2a0b62d5a9';
        var collection = 'DisasterViewModel';

        factory.getAllReports = function(location) {

            var fiveHoursBefore = moment().add("hours", -5).toISOString();
            var now = moment().toISOString();

            var query = {
                "CreatedDate":
                    {
                        $gte: fiveHoursBefore,
                        $lt: now
                    }
                //,
                //"Location":
                //    {
                //        $near:
                //            {
                //                $geometry: { type: "Point", coordinates: [location.lat, location.lng] },
                //                $maxDistance: 500
                //            }
                //    }
            };
            
            var url = baseUrl + db + '/collections/' + collection + '?apiKey=' + apiKey + '&q=' + JSON.stringify(query);            
            return $http.get(url).error(function(err) {
                alert("http error: " + err);
            }).success(function (d) {
                alert("got data: " + d);
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