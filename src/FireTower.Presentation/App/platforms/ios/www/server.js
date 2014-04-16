/**
 * Created by Kmilo on 3/24/14.
 */
var http = require('http'),
    fs = require('fs');


http.createServer(function(req, res) {
    var path = "." + req.url;
    if(path=="./") path = "./index.html";

    function stripQueryStringAndHashFromPath(url) {
        return url.split("?")[0].split("#")[0];
    }

    path = stripQueryStringAndHashFromPath(path);

    fs.readFile(path, function(error, content) {
        if (error) {
            console.log("Requested " + path + " but it couldn't be found.");
            res.writeHead(500);
            res.end();
        } else {
            var contentType = 'text/html';
            if(contains(path,".js")){
                contentType = "text/javascript";
            }
            if(contains(path,".css")){
                contentType = "text/css";
            }
            res.writeHead(200, {
                'Content-Type': contentType
            });
            res.end(content, 'utf-8');
        }
    });

}).listen(3000);

console.log("Test server listening on port 3000...");

var contains = function(stringToSearch, containsThis) {
    return (stringToSearch.indexOf(containsThis) != -1);
};