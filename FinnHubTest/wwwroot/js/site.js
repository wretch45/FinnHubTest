// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

var errorMessage = $("#errorMessage");

function displayMessage(message, isSuccess = false) {
    if (isSuccess) {
        errorMessage.removeClass('alert-danger').addClass('alert-success');
    } else {
        errorMessage.removeClass('alert-success').addClass('alert-danger');
    }
    errorMessage.text(message);
    errorMessage.show();
    setTimeout(function(){ errorMessage.hide(); }, 3000);
}
