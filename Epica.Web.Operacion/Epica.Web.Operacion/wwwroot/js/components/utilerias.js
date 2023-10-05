function SoloNumeros(e) {
    var charCode = (e.which) ? e.which : event.keyCode;
    if (String.fromCharCode(charCode).match(/[^0-9]/g))
        return false;
}
function SoloLetras(e) {
    var keyCode = e.keyCode || e.which;
    var regex = /^[A-Za-zÀÁàáÒÓòóÈÉèéÌÍìíÙÚùúÑñ? ]+$/;

    //Validate TextBox value against the Regex.
    var isValid = regex.test(String.fromCharCode(keyCode));
    if (!isValid) {
        return false;
    }

    return isValid;
}

function SinCaracteresEspeciales(e) {
    var keyCode = e.keyCode || e.which;
    var regex = /^[A-Za-z0-9ÀÁàáÒÓòóÈÉèéÌÍìíÙÚùúÑñ? ]+$/;

    //Validate TextBox value against the Regex.
    var isValid = regex.test(String.fromCharCode(keyCode));
    if (!isValid) {
        return false;
    }

    return isValid;

}

function validateNumbersInput(inputElement) {
    var inputValue = inputElement.value;
    var cleanValue = inputValue.replace(/[^0-9? ]/g, '');

    if (cleanValue !== inputValue) {
        inputElement.value = cleanValue;
    }
}

function validateInput(inputElement) {
    var inputValue = inputElement.value;
    var cleanValue = '';
    var hasSpace = false;

    for (var i = 0; i < inputValue.length; i++) {
        var char = inputValue[i];

        if (/[a-zA-ZÀÁàáÒÓòóÈÉèéÌÍìíÙÚùúÑñ]/.test(char) && cleanValue.length < 40) {
            cleanValue += char;
            hasSpace = false;
        } else if (char === ' ' && !hasSpace) {
            cleanValue += char;
            hasSpace = true;
        }
    }

    if (cleanValue.length < 3) {
        cleanValue = cleanValue.slice(0, 3);
    } else if (cleanValue.length > 40) {
        cleanValue = cleanValue.slice(0, 40);
    }

    inputElement.value = cleanValue;
}

function validateMontoInput(inputElement) {
    var inputValue = inputElement.value;

    var cleanValue = inputValue.replace(/[^0-9.]/g, '');

    cleanValue = cleanValue.replace(/(\..*)\./g, '$1');

    inputElement.value = cleanValue;
}

function validateClaveInput(inputElement) {
    var inputValue = inputElement.value;
    var cleanValue = '';

    for (var i = 0; i < inputValue.length; i++) {
        var char = inputValue[i];

        if (/^[a-zA-Z0-9ÀÁàáÒÓòóÈÉèéÌÍìíÙÚùúÑñ? ]*$/.test(char)) {
            cleanValue += char;
        }
    }

    inputElement.value = cleanValue;
}