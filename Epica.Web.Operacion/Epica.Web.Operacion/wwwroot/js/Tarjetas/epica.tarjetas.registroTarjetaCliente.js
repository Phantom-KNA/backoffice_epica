"use strict";

var Init = function () {
    var init = function () {
        const selectAnioVigencia = document.getElementById('anioVigencia');

        const currentYear = new Date().getFullYear();
        for (let year = currentYear; year <= 2100; year++) {
            const option = document.createElement('option');
            option.text = year.toString();
            option.value = year.toString();
            selectAnioVigencia.appendChild(option);
        }

        const selectMesVigencia = document.getElementById('mesVigencia');

        for (let month = 1; month <= 12; month++) {
            const option = document.createElement('option');
            const monthString = month.toString().padStart(2, '0');
            option.text = monthString;
            option.value = monthString;
            selectMesVigencia.appendChild(option);
        }

        $(document).on('click', '#btnGuardarRegistroTarjeta', function (e) {
            toastr.success('Registro guardado exitosamente', "");
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