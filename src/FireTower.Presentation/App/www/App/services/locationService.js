angular.module('firetower')
    .factory('LocationService', ['cache', '$q', '$location', function (cache, $q) {

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
            var lastPosition = cache.get("lastPosition");
            def.resolve(lastPosition);
            if (navigator.geolocation) {                    
                navigator.geolocation.getCurrentPosition(function (position) {
                    var newPosition = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude
                    };
                    cache.set("lastPosition", newPosition);
                    def.resolve(newPosition);                    
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