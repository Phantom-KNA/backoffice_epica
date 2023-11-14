var table;
var datatable;
var filterAccount;

function ResetContrasenaEmail(Email, ID) {

    Swal.fire({
        title: 'Enviar Reseteo de Contraseña por Correo Electrónico',
        text: "¿Estás seguro que deseas enviar el reseteo de contraseña para este cliente?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: siteLocation + 'Clientes/EnvioContrasenaCorreo',
                async: true,
                cache: false,
                type: 'POST',
                data: { correo: Email, id : ID },
                success: function (data) {

                    datatable.ajax.reload();
                    Swal.fire(
                        'Envío Reseteo Contraseña',
                        'Se ha enviado la petición con éxito.',
                        'success'
                    )
                },
                error: function (xhr, status, error) {
                    Swal.fire(
                        'Envío Reseteo Contraseña',
                        'Hubo un problema al realizar esta solicitud. Inténtalo más tarde.',
                        'danger'
                    )
                }
            });
        }
    })

}

function ResetContrasenaTelefono(Telefono, ID) {

    Swal.fire({
        title: 'Enviar Reseteo de Contraseña por SMS',
        text: "¿Estás seguro que deseas enviar el reseteo de contraseña para este cliente?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: siteLocation + 'Clientes/EnvioContrasenaTelefono',
                async: true,
                cache: false,
                type: 'POST',
                data: { telefono: Telefono, id: ID },
                success: function (data) {

                    datatable.ajax.reload();
                    Swal.fire(
                        'Envio Reseteo Contraseña',
                        'Se ha enviado la petición con éxito.',
                        'success'
                    )
                },
                error: function (xhr, status, error) {
                    Swal.fire(
                        'Envío Reseteo Contraseña',
                        'Hubo un problema al realizar esta solicitud. Inténtalo más tarde.',
                        'danger'
                    )
                }
            });
        }
    })

}

$(document).on("click", "#Redireccion", function () {
    KTApp.showPageLoading();
});