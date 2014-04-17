﻿angular.module('firetower')
    .service('settings', [function () {

        var baseUrl = 'http://firetowerapidev.apphb.com';
        if (document.location.hostname == "localhost")
            baseUrl = 'http://localhost:38397';

        console.log("Using baseUrl " + baseUrl);
        
        return {
            baseUrl : baseUrl
        };
    }]);