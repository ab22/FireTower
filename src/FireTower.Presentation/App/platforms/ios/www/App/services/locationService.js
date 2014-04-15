angular.module('firetower')
    .factory('LocationService', ['$q', '$location', function ($q) {

        var getLocationAddress = function (lat, lng) {
            var def = $q.defer();
            var geocoder = new google.maps.Geocoder();            
            var latlng = new google.maps.LatLng(lat, lng);
            geocoder.geocode({ 'latLng': latlng }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    if (results[1]) {
                        def.resolve(results[1].formatted_address);
                    } else {
                        alert('No results found');
                        def.reject('No results found');
                    }
                } else {
                    alert('Geocoder failed due to: ' + status);
                    def.reject('Geocoder failed due to: ' + status);
                }
            });
            return def.promise;
        };

        var getCurrentPosition = function() {
            var def = $q.defer();
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    var data = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude
                    };
                    def.resolve(data);                    
                }, function (err) {
                    debugger;
                    def.reject(err);
                });
            }
            return def.promise;
        };
        
        return {
            getLocationAddress: getLocationAddress,
            getCurrentPosition: getCurrentPosition
        };
    }]);