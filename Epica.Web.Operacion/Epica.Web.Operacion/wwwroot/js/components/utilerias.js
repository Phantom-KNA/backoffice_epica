function SoloNumeros(e) {
    var charCode = (e.which) ? e.which : event.keyCode;
    if (String.fromCharCode(charCode).match(/[^0-9]/g))
        return false;
}
function SoloLetras(e) {
    var keyCode = e.keyCode || e.which;
    var regex = /^[A-Za-z? ]+$/;

    //Validate TextBox value against the Regex.
    var isValid = regex.test(String.fromCharCode(keyCode));
    if (!isValid) {
        return false;
    }

    return isValid;
}

function SinCaracteresEspeciales(e) {
    var keyCode = e.keyCode || e.which;
    var regex = /^[A-Za-z0-9? ]+$/;

    //Validate TextBox value against the Regex.
    var isValid = regex.test(String.fromCharCode(keyCode));
    if (!isValid) {
        return false;
    }

    return isValid;

}