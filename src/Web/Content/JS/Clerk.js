$(document).ready(function () {
    $('div[data-remote="true"]').each(function (index, element) {
        var jqElement = $(element);
        var elementId = element.id;
        var url = jqElement.attr('data-url');
        var tmplId = jqElement.attr('data-template');
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