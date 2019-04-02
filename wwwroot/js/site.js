// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
var pat;

function setupNavigation(smart) {

    var impressionNode = $('section[data-section-name="Impression"]');
    var URL = "/FHIR/GetDiagnosticReport/";
    var procedureNode = $('section[data-section-name="Procedure"]');
    if (impressionNode.length > 0) {
        $.ajax({
            url: URL,
            type: 'POST',
            data: {
                AccessionId: "a819497684894126",
                Title: procedureNode.children()[0].innerHTML

            },
            success: function (data) {
                if (data != null) {
                    impressionNode.children()[1].children.Impression.value = data.conclusion.split(': ')[1];
                }
            },
            error: function (data) {
                console.log(data);
                console.log(URL);
            }
        });
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
        //var patient = smart.patient;
        //var pt = patient.read();
        //$.when(pt).done(function (patient) {

        //    var sub = {
        //        "reference": "Patient/" + patient.id
        //    }
        //    //var res = {
        //    //    "status": "registered",
        //    //    "started": "2008-12-20T18:31:21-05:00",
        //    //    "issued": "2008-12-20T18:31:21-05:00",
        //    //    "subject": sub,
        //    //    "resourceType": "ImagingStudy"
        //    //};

            

        //    //smart.api.create({ type: "ImagingStudy", data: res }).then(function () {
        //    //    console.log("Here");
        //    //    var obv = smart.patient.api.fetchAll({
        //    //        type: 'ImagingStudy'
        //    //    });
        //    //    $.when(obv).done(function (studies) {
        //    //        console.log(studies)
        //    //    });
        //    //});

        //});
        
   
    }


}

var smartVal;
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
    console.log(pat);
    var pdf;
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
            console.log("Success?");
            var doc = {
                "contentType": "application/pdf",
                "language":"en",
                "data": data,
                "title": title,
            }
            var coding = {
                "code": "51990-0",
                "display": "Basic Metabolic Panel",
                "system": "http://loinc.org"
            }
            var code = {
                "coding": [coding]
            }
            var sub = {
                "reference": "Patient/" + pat.ChildNodes[3].Result
            }
            var res = {
                "status": "final",
                "code": code,
                "effectiveDateTime": "2008-12-20T18:31:21-05:00",
                "issued": "2008-12-20T18:31:21-05:00",
                "subject": sub,
                "resourceType": "DiagnosticReport",
                "presentedForm": doc
            };

            smartVal.api.create({ type: "DiagnosticReport", data: res }).then(function () {
                console.log("Here");
            });
            $('#SuccessAlert').removeAttr('hidden');

        },
        error: function (data) {
            console.log(data);
            $('#FailureAlert').removeAttr('hidden');
        }
    });

});


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
                    var pt = patient.read();
                    var obv = smart.patient.api.fetchAll({
                        type: 'DiagnosticReport'
                    });
                    $.when(pt, obv).fail(onError);
                    $.when(pt, obv).done(function (patient, dr) {
                        console.log(patient);
                        console.log(dr);
                        pat = patient;
                        var patientNode = $('section[data-section-name="Patient Information"]')[0];
                        patientNode.children[1].children[1].value = patient.name[0].given + " " + patient.name[0].family;
                        patientNode.children[2].children[1].value = patient.gender;
                        patientNode.children[3].children[1].value = patient.birthDate;
                        patientNode.children[4].children[1].value = patient.id;
                        //patientNode.children[5].children[1].value = patient.
                        patientNode.children[6].children[1].value = patient.telecom[0].value;

                        //$.each(dr, function (val) {
                        //    $('#patientSelected').append('<option value="' + val.value + '">' + val.value + '</option>');
                        //});
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



