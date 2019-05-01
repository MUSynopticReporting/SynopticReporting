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
function setupNavigation(smart) {

    var impressionNode = $('section[data-section-name="Impression"]');
    var URL = "/FHIR/GetDiagnosticReport/";
    if (impressionNode.length > 0) {
        $('section[data-section-name="Patient Information"]').removeAttr('hidden');
    } else {
        $('#templateNav').removeAttr('hidden');
        $.ajax({
            url: '/Home/FindFiles',
            type: 'POST',
            async: 'false',
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                // Get a list of file names and append each to view list
                $.each(data, function (index, val) {
                    var sub = val.substring(0, val.length - 5);
                    $('#viewSelected').append('<option value="' + sub + '">' + sub + '</option>');
                });
            },
            error: function (data, status, error) {
                console.log(data);
                console.log(status);
                console.log(error);
            }
        });        
   
    }


}

var smartVal;
var pract;
$("#submit").click(function submit(e) {
    //document.getElementById('T3_2').value to get 
    var procedureNode = $('section[data-section-name="Procedure"]')[0];
    //console.log(procedureNode.children()[0].innerHTML);
    var title = procedureNode.children[0].innerHTML;
    var patNode = $('section[data-section-name="Patient Information"]')[0];
    var pat = getTree(patNode);
    var procedure = getTree(procedureNode);
    var clinicalNode = $('section[data-section-name="Clinical information"]')[0];
    var clinical = getTree(clinicalNode);
    var comparisonNode = $('section[data-section-name="Comparison"]')[0];
    var comparison = getTree(comparisonNode);
    var findingsNode = $('section[data-section-name="Findings"]')[0];
    var findings = getTree(findingsNode);
    var impressionNode = $('section[data-section-name="Impression"]')[0];
    var impression = getTree(impressionNode);
    var pdfDoc;
    var xmlDoc;
    $.ajax({
        url: 'Create',
        type: 'POST',
        data: {
            Location: window.location.href,
            title: title,
            Procedure: procedure,
            ClinicalInformation: clinical,
            Comparison: comparison,
            Findings: findings,
            PatientInfo: pat,
            Impression: impression
        },
        success: function (data) {
            pdfDoc = {
                "contentType": "application/pdf",
                "language":"en",
                "data": data,
                "title": title,
            }
            xmlDoc = {
                "contentType": "text/xml",
            }
            var coding = {
                "code": "18748-4",
                "display": "Diagnostic Imaging Report",
                "system": "http://loinc.org"
            }
            var code = {
                "coding": [coding]
            }
            var catCoding = {
                "code": "RAD",
                "display": "Radiology",
                "system": "http://terminology.hl7.org/CodeSystem/v2-0074"
            }
            var cat = {
                "coding": [catCoding]
            }
            var sub = {
                "reference": "Patient/" + pat.ChildNodes[3].Result
            }
            var perf = {
                "reference": "Practitioner/" + pract.id
            }
            var today = new Date();
            //var presented = [
            //    pdfDoc,
            //    xmlDoc
            //]
            console.log(pdfDoc);
            console.log(xmlDoc);

            var res = {
                "status": "final",
                "code": code,
                "effectiveDateTime": today,
                "issued": today,
                "subject": sub,
                "resourceType": "DiagnosticReport",
                "presentedForm": pdfDoc,
                "conclusion": impression.ChildNodes[0].Result,
                "performer": perf
            };

            smartVal.api.create({ type: "DiagnosticReport", data: res }).then(function () {
            });
            $('#SuccessAlert').removeAttr('hidden');


        },
        error: function (data) {
            console.log(data);
            $('#FailureAlert').removeAttr('hidden');
        }
    });

});

function json2xml(o, tab) {
    var toXml = function (v, name, ind) {
        var xml = "";
        if (typeof (v) == "object") {
            var hasChild = false;
            for (var m in v) {
                if (m.charAt(0) == "@") {
                    xml += " " + m.substr(1) + "=\"" + v[m].toString() + "\"";
                } else{
                    hasChild = true;
                }
            }
            if (hasChild) {
                for (var m in v) {
                    if (m == "name") {
                        xml += "<" + v[m] + ">" + v['value'] + "";
                    }
                }
            }
        }
        return xml;
    }, xml = "";
    for (var m in o) {
        xml += toXml(o[m], m, "");
    }
    return " "+xml+" ";
}


function loadView() {
    var e = document.getElementById("viewSelected");
    var strUser = e.options[e.selectedIndex].value;
    window.location.href = "/Home/" + "LoadTemplate?path=" + strUser;
    $.ajax({
        url: '/Home/LoadTemplate',
        data: {
            path: strUser
        },
        success: function (data) {
            console.log("Yay");
        }
    })
}
document.getElementById("viewLoader").addEventListener("click", loadView);
//XMLDocument docXML = new XMLDocument(loadView);   //I don't think I can do this 


// Ok template 16 sucks
function isUnit(input) {
    var unitList = ["cm", "cm3" ];
    for (var i = 0; i < unitList.length; i++) {
        var whatever = input.indexOf(unitList[i]);
        if (whatever !== -1)
            return true;
    }
    return false;
}

// Recursive function to return object with the following properties:
/*
 * Type TemplateViewModel
 * IsLeaf: Boolean - If true then header is the label and results is not null, 
 *                      otherwise header is category header and ChildNodes is not null
 * Header: String - Either label or header depending on IsLeaf
 * Result: String - Contents of Text field or alternative if IsLeaf == True
 * ChildNodes: List<TemplateViewModel> - If IsLeaf == False then go through list of children for node
 *                                       For each child, append a new call of getTree to ChildNodes
 */
function getTree(node) {
    var output = {};
    if (node.nodeName == "SECTION") {
        output.IsLeaf = false;
        output.Header = node.children[0].innerText;
        output.Result = null;
        output.ChildNodes = {};
        for (var i = 1; i < node.childElementCount; i++) {
            var temp = getTree(node.children[i]);
            output.ChildNodes[i - 1] = temp;
        }
    } else {
        output.IsLeaf = true;
        output.ChildNodes = null;
        if (node.children[0].tagName == "TEXTAREA") {
            output.Header = node.children[0].name;
            output.Result = node.children[0].value;
        } else {
            output.Header = node.children[0].textContent;
            output.Result = node.children[1].value;
        }

    }    
        
    return output;
}
var patient;
var user;
(function (window) {
    window.extractData = function () {
        var ret = $.Deferred();

        function onError() {
            console.log('Loading error', arguments);
            ret.reject();
        }
        function onError2() {

        }
        function onReady(smart) {
            smartVal = smart;
            if (smart.hasOwnProperty('patient')) {
                setupNavigation(smart);
                var impressionNode = $('section[data-section-name="Impression"]');
                if (impressionNode.length > 0) {
                    patient = smart.patient;
                    user = smart.user;
                    var pt = patient.read();
                    var usr = user.read();
                    $.when(pt, usr).fail(onError);
                    $.when(pt, usr).done(function (patient, user) {
                        console.log(patient);
                        console.log(user);
                        pract = user;
                        var patientNode = $('section[data-section-name="Patient Information"]')[0];
                        patientNode.children[1].children[1].value = patient.name[0].given + " " + patient.name[0].family;
                        patientNode.children[2].children[1].value = patient.gender;
                        patientNode.children[3].children[1].value = patient.birthDate;
                        patientNode.children[4].children[1].value = patient.id;
                        patientNode.children[6].children[1].value = patient.telecom[0].value;

                    });

                }
            } else {
                onError();
                // Replace onError with setupNavigation() here for testing if you want
            }
        }
        FHIR.oauth2.ready(onReady, onError2);
        return ret.promise();
    };
    window.drawVisualization = function (p) {
        console.log("Draw Visualization");
    };
})(window);



