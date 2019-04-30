//This file is part of MUSynopticReporting.

//MUSynopticReporting is free software: you can redistribute it and / or modify
//it under the terms of the GNU Affero General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//MUSynopticReporting is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//along with MUSynopticReporting.If not, see < https://www.gnu.org/licenses/>.

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

