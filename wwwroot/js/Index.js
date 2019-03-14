$('document').ready(function () {
    $('#submit').prop("disabled", true);

});

(function (window) {
    window.extractData = function () {
        var ret = $.Deferred();
        function onError() {
            console.log('Loading error', arguments);
            ret.reject();
        }
        function onReady(smart) {
            if (smart.hasOwnProperty('patient')) {
                console.log("Success");
            } else {
                onError();
            }
        }
        FHIR.oauth2.ready(onReady, onError);
        return ret.promise();
    };
    window.drawVisualization = function (p) {
        console.log("Draw Visualization");
    };


})(window);

