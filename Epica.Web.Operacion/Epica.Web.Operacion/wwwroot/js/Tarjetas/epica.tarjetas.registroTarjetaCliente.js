"use strict";

const selectYearVigencia = document.getElementById('yearVigencia');
const yearVigencia = selectYearVigencia.value;

$(document).on('click', '#btnGuardarTarjeta', function (e) {
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
$(document).on('click', '#btnCerrarModal, #btnCerrarModal2, .modal-close', function (e) {
    $('#confirmModal').modal('hide');
});

$(document).on('click', '#btnBuscarCliente', function () {
    const inputNombreCliente = document.getElementById('nombreCliente');
    const nombreCliente = inputNombreCliente.value;

    toastr.info("Localizando clientes.");

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

            toastr.success("Clientes localizados.");
        },
        error: function (error) {
            toastr.error("No se pudieron localizar los clientes.");
        }
    });
});