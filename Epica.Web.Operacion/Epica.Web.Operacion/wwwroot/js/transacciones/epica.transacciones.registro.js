"use strict";

var Init = function () {
    var init = function () {

        $('.form-select').select2({
        });

        $(document).on('click', '#btnGuardarTransaccion', function (e) {
            var requiredFields = document.querySelectorAll('[required]');

            var allFieldsFilled = true;

            requiredFields.forEach(function (field) {
                if (!field.value) {
                    allFieldsFilled = false;
                }
            });

            if (allFieldsFilled) {
                $(this).text('Registrando información');
                toastr.info('Registrando información', "");
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
jQuery(document).ready(function () {
    Init.init();
});