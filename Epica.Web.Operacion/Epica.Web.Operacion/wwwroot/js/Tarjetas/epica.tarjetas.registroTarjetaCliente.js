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
        // Realiza la validación del token antes de abrir el modal de confirmación
        Swal.fire({
            title: '<span class="swal-title-custom"><b>Valida tu identidad</b></span>',
            text: 'Por favor, ingrese su token y código de seguridad:',
            html:
                '<label for="swal-input1" class="swal-label"><b>Token:</b></label>' +
                '<input id="swal-input1" class="swal2-input" style="font-size:14px;width:250px"  placeholder="Token">' +
                '<div style="margin-top: 20px;"></div>' +
                '<label for="swal-input2" class="swal-label"><b>Código de Seguridad:</b></label>' +
                '<input id="swal-input2" class="swal2-input" style="font-size:14px;width:250px" type="password" placeholder="Código de Seguridad">',
            showCancelButton: true,
            confirmButtonColor: '#0493a8',
            confirmButtonText: 'Aceptar',
            cancelButtonText: 'Cancelar',
            preConfirm: () => {
                const swalInput1 = Swal.getPopup().querySelector('#swal-input1').value;
                const swalInput2 = Swal.getPopup().querySelector('#swal-input2').value;
                return [swalInput1, swalInput2];
            }
        }).then((result) => {
            if (result.isConfirmed) {
                const [tokenInput, codigoInput] = result.value;

                // Realiza la validación del token y código de seguridad
                $.ajax({
                    url: '/Autenticacion/ValidarTokenYCodigo',
                    async: true,
                    cache: false,
                    type: 'POST',
                    data: { token: tokenInput, codigo: codigoInput },
                    success: function (validationResult) {
                        if (validationResult.mensaje === true) {
                            // La validación fue exitosa, abre el modal de confirmación
                            $('#confirmModal').modal('show');
                        } else {
                            toastr.error('Token o código de seguridad incorrectos', "");
                        }
                    },
                    error: function () {
                        toastr.error('Hubo un error en la validación del token y código de seguridad', "");
                    }
                });
            }
        });
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