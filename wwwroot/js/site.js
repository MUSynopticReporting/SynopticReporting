// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

$(document).ready(function () {
    var impressionNode = $('section[data-section-name="Impression"]');
    var URL = "/FHIR/GetDiagnosticReport/";
    var procedureNode = $('section[data-section-name="Procedure"]');
    if (impressionNode.length > 0) {
        $.ajax({
            url: URL,
            type: 'GET',
            data: {
                AccessionId: "a819497684894126",
                Title: procedureNode.children()[0].innerHTML

            },
            success: function (data) {
                console.log(data);
                if (data != null) {
                    impressionNode.children()[1].children.Impression.value = data.conclusion.split(': ')[1];
                }
            },
            error: function (data) {
                console.log(data);
                console.log(URL);
            }
        });

    }
});
$("#submit").click(function (e) {
//document.getElementById('T3_2').value to get 
    var procedureNode = $('section[data-section-name="Procedure"]');
    var multi = true;
    console.log(procedureNode.children()[0].innerHTML);
    var title = procedureNode.children()[0].innerHTML;
    var procedure = [];
    var clinical = [];
    var comparison = [];
    var findings = [];
    var impression = [];
    var findingsSections = [];
    for (var i = 1; i < procedureNode.children().length; i++) {
        var procedureId = procedureNode.children()[i].lastElementChild.id;
        console.log(procedureNode.children()[i].lastElementChild.name + ": " + document.getElementById(procedureId).value);
        procedure.push(procedureNode.children()[i].lastElementChild.name + ": " + document.getElementById(procedureId).value)
    }
    var clinicalNode = $('section[data-section-name="Clinical information"]');
    for (var i = 1; i < clinicalNode.children().length; i++) {
        var clinicalId = clinicalNode.children()[i].lastElementChild.id;
        console.log(clinicalNode.children()[i].lastElementChild.name + ": " + document.getElementById(clinicalId).value);
        clinical.push(clinicalNode.children()[i].lastElementChild.name + ": " + document.getElementById(clinicalId).value)
    }
    var comparisonNode = $('section[data-section-name="Comparison"]');
    for (var i = 1; i < comparisonNode.children().length; i++) {
        var comparisonId = comparisonNode.children()[i].lastElementChild.id;
        console.log(comparisonNode.children()[i].lastElementChild.name + ": " + document.getElementById(comparisonId).value);
        comparison.push(comparisonNode.children()[i].lastElementChild.name + ": " + document.getElementById(comparisonId).value)
    }
    // Maybe just naively assume each first level child is going to be its own section?
    // Deal with this garbage next semester lol
    var findingsNode = $('section[data-section-name="Findings"]');
    for (var i = 1; i < findingsNode.children().length; i++) {
        var findingsSection = findingsNode.children()[i];
        findingsSections.push(findingsNode.children()[i].children[0].textContent)
        console.log("Sub Section: " + findingsNode.children()[i].children[0].textContent);
        var tempList = [];
        for (var j = 1; j < findingsSection.childElementCount; j++) {
            if (findingsSection.children[j].childElementCount > 0) {
                for (var k = 0; k < findingsSection.children[j].childElementCount; k++) {
                    var currentFinding = findingsSection.children[j].children[k];
                    if (currentFinding.tagName == "INPUT") {
                        //Check if innerText does not contain a colon, if so then append during controller call
                        //Also add a section for units: If children[k + 1] is defined and part of a list of units, append to end
                        console.log((i - 1) + " " + findingsSection.children[j].children[k - 1].innerText + " " + document.getElementById(currentFinding.id).value);
                        if ((k + 1 < findingsSection.children[j].childElementCount) && isUnit(findingsSection.children[j].children[k + 1].innerText)) {
                            //console.log("Unit = " + findingsSection.children[j].children[k + 1].innerText);
                            tempList.push(findingsSection.children[j].children[k - 1].innerText + " " + document.getElementById(currentFinding.id).value + " " + findingsSection.children[j].children[k + 1].innerText);
                        } else {
                            tempList.push(findingsSection.children[j].children[k - 1].innerText + " " + document.getElementById(currentFinding.id).value);
                        }

                        //Check if matchesUnitList(findingsSection.children[j].children[k + 1]) is true, if so append to end

                    } else if (findingsSection.children[j].lastElementChild.tagName == "TEXTAREA") {
                        console.log("K is: " + k);
                        console.log(findingsSection);
                        tempList.push(findingsSection.children[0].textContent + ": " + findingsSection.children[j].lastElementChild.value)
                    }
                }
            } else {
                multi = false;
                var currentFinding = findingsSection.children[j];
                if (currentFinding.tagName == "INPUT") {
                    //Check if innerText does not contain a colon, if so then append during controller call
                    //Also add a section for units: If children[k + 1] is defined and part of a list of units, append to end
                    console.log((i - 1) + " " + findingsSection.children[j - 1].innerText + " " + document.getElementById(currentFinding.id).value);
                    if ((j + 1 < findingsSection.childElementCount) && isUnit(findingsSection.children[j + 1].innerText)) {
                        //console.log("Unit = " + findingsSection.children[j].children[k + 1].innerText);
                        tempList.push(findingsSection.children[j - 1].innerText + " " + document.getElementById(currentFinding.id).value + " " + findingsSection.children[j + 1].innerText);
                    } else {
                        tempList.push(findingsSection.children[j - 1].innerText + " " + document.getElementById(currentFinding.id).value);
                    }

                    //Check if matchesUnitList(findingsSection.children[j].children[k + 1]) is true, if so append to end

                } else if (findingsSection.children[j].lastElementChild.tagName == "TEXTAREA") {
                    console.log(findingsSection);
                    tempList.push(findingsSection.children[0].textContent + ": " + findingsSection.children[j].lastElementChild.value)
                }
            }
        }
        findings.push(tempList);
    }
    var impressionNode = $('section[data-section-name="Impression"]');
    for (var i = 1; i < impressionNode.children().length; i++) {
        var impressionId = impressionNode.children()[i].lastElementChild.id;
        console.log(impressionNode.children()[i].lastElementChild.name + ": " + document.getElementById(impressionId).value);
        impression.push(impressionNode.children()[i].lastElementChild.name + ": " + document.getElementById(impressionId).value)
    }
    $.ajax({
        url: multi ? 'Create' : 'CreateSingle',
        type: 'POST',
        data: {
            title: title,
            Procedure: procedure,
            ClinicalInformation: clinical,
            Comparison: comparison,
            Findings: findings,
            Impression: impression,
            FindingsList: findingsSections
        },
        success: function (data) {
            console.log("Success?");
        },
        error: function (data) {
            console.log(data);
        }
    });
});


function loadView() {
    var e = document.getElementById("viewSelected");
    var strUser = e.options[e.selectedIndex].value;
    window.location.href = "/Home/" + "LoadTemplate?path=" + strUser;
    $.ajax({
        url: 'Home/LoadTemplate',
        data: {
        path: strUser
        },
        success: function (data) {
            console.log("Yay");
        }
    })
}
document.getElementById("viewLoader").addEventListener("click", loadView);



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