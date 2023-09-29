"use strict";
toastr.options.preventDuplicates = true;

function validateInput(inputElement) {
    var inputValue = inputElement.value;
    var cleanValue = inputValue.replace(/[^A-Za-z]/g, '');

    if (cleanValue !== inputValue) {
        inputElement.value = cleanValue;
    }
}

function validateNumbersInput(inputElement) {
    var inputValue = inputElement.value;
    var cleanValue = inputValue.replace(/[^0-9]/g, '');

    if (cleanValue !== inputValue) {
        inputElement.value = cleanValue;
    }
}

function validateRFC(inputElement) {
    var rfcValue = inputElement.value;
    var rfcPattern = /^[A-Za-z]{4}\d{6}[A-Za-z\d]{3}$/;

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
                    if (field.value.length < 3 || field.value.length > 20) {
                        var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' debe tener entre 3 y 20 caracteres';
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