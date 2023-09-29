"use strict";
toastr.options.preventDuplicates = true;

function validateLettersInput(inputElement) {
    var inputValue = inputElement.value;
    var cleanValue = '';

    for (var i = 0; i < inputValue.length; i++) {
        var char = inputValue[i];
        if (/[a-zA-Z]/.test(char) && cleanValue.length < 20) {
            cleanValue += char;
        }
    }

    if (cleanValue.length < 3) {
        cleanValue = cleanValue.slice(0, 3);
    } else if (cleanValue.length > 20) {
        cleanValue = cleanValue.slice(0, 20);
    }

    inputElement.value = cleanValue;
}

function validateNumbersInput(inputElement) {
    var inputValue = inputElement.value;
    var cleanValue = '';

    var prevChar = ''; 

    for (var i = 0; i < inputValue.length; i++) {
        var char = inputValue[i];
        if (!isNaN(char) && char !== prevChar) {
            cleanValue += char;
            prevChar = char; 
        }
    }

    inputElement.value = cleanValue;
}


const selectYearVigencia = document.getElementById('yearVigencia');
const yearVigencia = selectYearVigencia.value;

$(document).on('click', '#btnGuardarTarjeta', function (e) {
    var requiredFields = document.querySelectorAll('[required]');

    var allFieldsFilled = true;
    var validationFailed = false;
    var firstErrorMessage = null;

    requiredFields.forEach(function (field) {
        if (!field.value) {
            allFieldsFilled = false;
            var errorMessage = field.getAttribute("data-error-message") || "Este campo";
            if (!firstErrorMessage) {
                firstErrorMessage = errorMessage + ' es obligatorio';
            }
        }
        if (field.id === "NumeroTarjeta") {
            if (field.value.length < 14 || field.value.length > 16) {
                var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                if (!firstErrorMessage) {
                    firstErrorMessage = ' Ingresa un número de Tarjeta válido';
                }
                validationFailed = true;
            }
        }
        if (field.id === "proxyNumber") {
            if (field.value.length < 11 || field.value.length > 11) {
                var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                if (!firstErrorMessage) {
                    firstErrorMessage = ' Ingresa un número de Proxy válido';
                }
                validationFailed = true;
            }
        }

        if (field.id === "nombreCliente" ) {
            if (field.value.length < 3 || field.value.length > 20) {
                var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                if (!firstErrorMessage) {
                    firstErrorMessage = errorMessage + ' debe tener entre 3 y 20 caracteres';
                }
                validationFailed = true;
            }
        }
    });

    if (!allFieldsFilled || validationFailed) {
        toastr.error(firstErrorMessage, "");
    } else {
        $('#confirmModal').modal('show');
    }
});
$(document).on('click', '#btnCerrarModal, #btnCerrarModal2, .modal-close', function (e) {
    $('#confirmModal').modal('hide');
});

$(document).on('click', '#btnBuscarCliente', function () {
    const inputNombreCliente = document.getElementById('nombreCliente');
    const nombreCliente = inputNombreCliente.value;

    toastr.info("Localizando clientes.").preventDuplicates;
    toastr.clear();

    $.ajax({
        url: "/Clientes/BuscarClientes",
        type: "POST",
        data: { nombreCliente: nombreCliente },
        success: function (results) {
            const selectClientes = document.getElementById('selectClientes');
            selectClientes.innerHTML = '';

            results.forEach(function (item) {
                const option = document.createElement('option');
                option.value = item.id;
                option.text = item.descripcion;
                selectClientes.appendChild(option);
            });

            toastr.success("Clientes localizados.").preventDuplicates;
        },
        error: function (error) {
            toastr.error("No se pudieron localizar los clientes.").preventDuplicates;
        }
    });
});

$(document).on('keydown', '#nombreCliente', function (e) {
    if (e.keyCode === 13) {
        e.preventDefault();
        $('#btnBuscarCliente').click();
    }
});