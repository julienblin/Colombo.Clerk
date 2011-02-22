$(document).ready(function () {

    $('#menu ul').supersubs({
        minWidth: 12,
        maxWidth: 27,
        extraWidth: 1
    }).superfish();

    $('div[data-url]').each(function (index, element) {
        var jqElement = $(element);
        var elementId = element.id;
        var url = jqElement.attr('data-url');
        var tmplId = jqElement.attr('data-template') || (elementId + 'Template');
        $.ajax({
            url: url,
            type: 'GET',
            contentType: 'application/json',
            success: function (data, status, xhr) {
                $("#" + tmplId).tmpl(data).appendTo("#" + elementId);
            },
            error: function (xhr, status, error) {
                alert(error);
            }
        });
    });
});