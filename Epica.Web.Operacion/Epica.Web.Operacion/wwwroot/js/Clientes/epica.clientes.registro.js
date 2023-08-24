"use strict";

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

            requiredFields.forEach(function (field) {
                if (!field.value) {
                    allFieldsFilled = false;
                }
            });

            if (allFieldsFilled) {
                $('#confirmModal').modal('show');
            } else {
                toastr.error('Por favor, complete todos los campos obligatorios', "");
            }
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