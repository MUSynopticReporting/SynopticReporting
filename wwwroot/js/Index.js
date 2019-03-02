$('document').ready(function () {
    $('#submit').prop("disabled", true);

});

var $table = $('#grid-data');

$(document).ready(function () {
    $.ajax({
        url: '/FHIRController/FindPatients',
        async: 'false',
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (data) {
            console.log(data);
            $('#grid-data').bootstrapTable({
                data: data
            });
        }
    })
})
$("#grid-data").bootgrid({
    ajax: true,
    post: function () {
        /* To accumulate custom parameter with the request object */
        return {
            id: "b0df282a-0d67-40e5-8558-c9e93b7befed"
        };
    },
    url: "/FHIRController/FindPatients",
    formatters: {
        "link": function (column, row) {
            return "<a href=\"#\">" + column.id + ": " + row.id + "</a>";
        }
    }
});
