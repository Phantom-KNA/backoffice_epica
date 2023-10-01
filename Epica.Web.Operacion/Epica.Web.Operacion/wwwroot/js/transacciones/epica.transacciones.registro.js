"use strict";
toastr.options.preventDuplicates = true;

function validateInput(inputElement) {
    var inputValue = inputElement.value;
    var cleanValue = '';
    var hasSpace = false;

    for (var i = 0; i < inputValue.length; i++) {
        var char = inputValue[i];

        if (/[a-zA-Z]/.test(char) && cleanValue.length < 40) {
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

function validateNumbersInput(inputElement) {
    var inputValue = inputElement.value;
    var cleanValue = inputValue.replace(/[^0-9]/g, '');

    if (cleanValue !== inputValue) {
        inputElement.value = cleanValue;
    }
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

        if (/^[a-zA-Z0-9]*$/.test(char)) {
            cleanValue += char;
        }
    }

    inputElement.value = cleanValue;
}

var Init = function () {
    var init = function () {

        $('.form-select').select2({
        });
        $('.datepicker-js').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            minYear: 1901,
            maxYear: parseInt(moment().format("YYYY"), 12),
            autoApply: true,
            locale: {
                format: 'DD/MM/YYYY',
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                weekLabel: "S"
            }
        });
        $(document).on('click', '#btnGuardarTransaccion', function (e) {
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

                if (field.id === "NoCuentaBeneficiario") {
                    if (field.value.length < 16 || field.value.length > 18) {
                        var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                        if (!firstErrorMessage) {
                            firstErrorMessage = ' Ingresa un número de Cuenta válido';
                        }
                        validationFailed = true;
                    }
                }

                if (field.id === "ClaveRastreo") {
                    if (field.value.length < 25 || field.value.length > 25) {
                        var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                        if (!firstErrorMessage) {
                            firstErrorMessage = ' Ingresa una clave de Rastreo válida';
                        }
                        validationFailed = true;
                    }
                }
                if (field.id === "FechaOperacion") {
                    var inputValue = field.value;

                    var minDate = new Date(2000, 0, 1);
                    var maxDate = new Date();

                    var inputDate = new Date(inputValue);

                    if (isNaN(inputDate.getTime()) || inputDate < minDate || inputDate > maxDate) {
                        var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                        if (!firstErrorMessage) {
                            firstErrorMessage = ' Ingresa una Fecha válida ';
                        }
                        validationFailed = true;
                    }
                }

                if (field.id === "Monto") {
                    var monto = parseFloat(field.value.replace(/,/g, ''));

                    if (isNaN(monto) || monto < 0.0001 || monto > 999999999) {
                        var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                        if (!firstErrorMessage) {
                            firstErrorMessage = ' Ingresa un Monto válido';
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
    }
    return {
        init: function () {
            init();
        }
    };
}();

$(document).ready(function () {
    Init.init();

    $('#confirmModal').on('hidden.bs.modal', function () {
    });
});