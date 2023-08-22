"use strict";

var Init = function () {
    var init = function () {
        const inputNombreCliente = document.getElementById('nombreCliente');
        const selectIdCliente = document.getElementById('idCliente');
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

    $(document).on('click', '#btnBuscarCliente', function () {
        const nombreCliente = inputNombreCliente.value;

        $.ajax({
            url: "/Tarjetas/BuscarClientes",
            type: "POST",
            data: { nombreCliente: nombreCliente },
            success: function (result) {
                selectIdCliente.innerHTML = "";

                result.forEach(function (cliente) {
                    const option = document.createElement('option');
                    option.text = cliente.Descripcion;
                    option.value = cliente.Id;
                    selectIdCliente.appendChild(option);
                });
            },
            error: function (error) {
                console.error("Error en la búsqueda de clientes: " + error);
            }
        });
    });
};

return {
    init: function () {
        init();
    }
};
}();

jQuery(document).ready(function () {
    Init.init();
});