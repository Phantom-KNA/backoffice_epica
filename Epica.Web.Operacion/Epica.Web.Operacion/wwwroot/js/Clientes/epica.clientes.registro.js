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

function validateAlphanumericInput(inputElement) {
    var inputValue = inputElement.value;
    var cleanValue = inputValue.replace(/[^a-zA-Z0-9]/g, '');

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

function validateRFC(inputElement) {
    var rfcValue = inputElement.value;
    var rfcPattern = /^[A-Z]{4}\d{6}[HM]{1}[A-Z]{5}[0-9]{2}$/;

    if (!rfcPattern.test(rfcValue)) {
        inputElement.value = ""; 
        toastr.error("RFC no válido. Debe seguir el formato correcto.", "");
    }
}

function validateCURP(inputElement) {
    var curpValue = inputElement.value;
    var curpPattern = /^[A-Z]{4}\d{6}[HM]{1}[A-Z]{5}[0-9]{2}$/;

    if (!curpPattern.test(curpValue)) {
        inputElement.value = "";
        toastr.error("CURP no válida. Debe seguir el formato correcto.", "");
    }
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

        $(document).on('click', '#btnGuardarCliente', function (e) {
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

                if (field.id === "Nombre" || field.id === "ApellidoPaterno" || field.id === "ApellidoMaterno" || field.id === "PaisNacimiento" || field.id === "Colonia" || field.id === "DelegacionMunicipio" || field.id === "CiudadEstado" || field.id === "Calle" || field.id === "PrimeraCalle" || field.id === "SegundaCalle" || field.id === "Puesto" || field.id === "Observaciones") {
                    if (field.value.length < 3 || field.value.length > 40) {
                        var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                        if (!firstErrorMessage) {
                            firstErrorMessage = ' Ingresa un nombre válido en el campo' + errorMessage;
                        }
                        validationFailed = true;
                    }
                }

                if (field.id === "Email") {
                    var emailValue = field.value;
                    var emailPattern = /[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}/;
                    if (!emailPattern.test(emailValue)) {
                        var errorMessage = field.getAttribute("data-error-message") || "Email";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' no es válido';
                        }
                        validationFailed = true;
                    }
                }

                if (field.id === "Telefono") {
                    var phoneNumber = field.value;
                    if (!/^\d{10}$/.test(phoneNumber)) {
                        var errorMessage = field.getAttribute("data-error-message") || "Teléfono Móvil";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' debe tener 10 dígitos';
                        }
                        validationFailed = true;
                    }
                }
                if (field.id === "Rfc") {
                    var rfc = field.value;
                    var rfcPattern = /^[A-Z&Ñ]{3,4}\d{6}[A-Z\d]{3}$/;

                    if (!rfcPattern.test(rfc)) {
                        var errorMessage = field.getAttribute("data-error-message") || "RFC";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' no es válido';
                        }
                        validationFailed = true;
                    }
                }
                if (field.id === "IngresoMensual") {
                    var monto = parseFloat(field.value.replace(/,/g, ''));

                    if (isNaN(monto) || monto < 0.0001 || monto > 9999999) {
                        var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                        if (!firstErrorMessage) {
                            firstErrorMessage = ' Ingresa un Monto válido';
                        }
                        validationFailed = true;
                    }
                }
                if (field.id === "MontoMaximo") {
                    var monto = parseFloat(field.value.replace(/,/g, ''));

                    if (isNaN(monto) || monto < 0.1 || monto > 999999999) {
                        var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                        if (!firstErrorMessage) {
                            firstErrorMessage = ' Ingresa un Monto válido';
                        }
                        validationFailed = true;
                    }
                }

                if (field.id === "Curp") {
                    var curp = field.value;
                    var curpPattern = /^[A-Z]{4}[0-9]{6}[H,M][A-Z]{5}[0-9]{2}$/;

                    if (!curpPattern.test(curp)) {
                        var errorMessage = field.getAttribute("data-error-message") || "CURP";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' no es válida';
                        }
                        validationFailed = true;
                    }
                }
                if (field.id === "IngresoMensual" || field.id === "MontoMaximo") {
                    var numericValue = parseFloat(field.value);
                    if (isNaN(numericValue) || numericValue < 100 || numericValue > 9999999) {
                        var errorMessage = field.getAttribute("data-error-message") || "Valor";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' debe estar entre 100 y 9999999';
                        }
                        validationFailed = true;
                    }
                }
                if (field.id === "Ine") {
                    var ineValue = field.value;
                    var inePattern = /^[A-Z]{6}\d{8}[HM][A-Z]{2}[A-Z0-9]{1}$/;

                    if (!inePattern.test(ineValue)) {
                        var errorMessage = field.getAttribute("data-error-message") || "INE";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' no es válido';
                        }
                        validationFailed = true;
                    }
                }
                if (field.id === "Fiel") {
                    var fielValue = field.value;

                    if (fielValue.trim() === "") {
                        return; 
                    }

                    var fielPattern = /^[a-zA-Z0-9+/=]+$/;

                    if (!fielPattern.test(fielValue)) {
                        var errorMessage = field.getAttribute("data-error-message") || "FIEL";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' no es válido';
                        }
                        validationFailed = true;
                    }
                }
                if (field.id === "CodigoPostal") {
                    var codigoPostalValue = field.value;
                    var codigoPostalPattern = /^\d{5}$/; 

                    if (!codigoPostalPattern.test(codigoPostalValue)) {
                        var errorMessage = field.getAttribute("data-error-message") || "Código Postal";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' no es válido';
                        }
                        validationFailed = true;
                    } else {
                        var codigoPostalNumerico = parseInt(codigoPostalValue, 10);
                        if (codigoPostalNumerico < 10000 || codigoPostalNumerico > 99999) {
                            var errorMessage = field.getAttribute("data-error-message") || "Código Postal";
                            if (!firstErrorMessage) {
                                firstErrorMessage = errorMessage + ' está fuera del rango permitido';
                            }
                            validationFailed = true;
                        }
                    }
                }
                if (field.id === "FechaNacimiento") {
                    var fechaNacimientoValue = field.value;
                    var fechaNacimiento = new Date(fechaNacimientoValue);

                    var fechaLimite = new Date();
                    fechaLimite.setFullYear(fechaLimite.getFullYear() - 18);

                    if (isNaN(fechaNacimiento.getTime()) || fechaNacimiento > fechaLimite || fechaNacimiento < new Date("1900-01-01")) {
                        var errorMessage = field.getAttribute("data-error-message") || "Fecha de Nacimiento";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' no es válida';
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