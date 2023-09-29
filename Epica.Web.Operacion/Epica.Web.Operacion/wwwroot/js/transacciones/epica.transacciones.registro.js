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

                if (field.id === "NombreOrdenante" || field.id === "Concepto") {
                    if (field.value.length < 3 || field.value.length > 20) {
                        var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' Ingresa un nombre válido';
                        }
                        validationFailed = true;
                    }
                }

                if (field.id === "NoCuentaBeneficiario") {
                    if (field.value.length < 16 || field.value.length > 18) {
                        var errorMessage = field.getAttribute("data-error-message") || "Este campo";
                        if (!firstErrorMessage) {
                            firstErrorMessage = errorMessage + ' Ingresa un número de cuenta válido';
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