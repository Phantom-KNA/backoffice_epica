(function ($) {
    var validator = $("#formAutenticacion").validate({
        rules: {
            Usuario: "required",
            Contrasenia: "required",
        },
        messages: {
            Usuario: "El usuario es requerido",
            Contrasenia: "La constraseña es requerida",
        }
    });
    //$.validator.messages.required = 'El campo es requerido';
    var eventosControl = function () {

        $("#btnSesion").click(function (e) {
            e.preventDefault();

            eventos.botonCargando();

            var form = $(this).closest('form');

            if (!$("#formAutenticacion").valid()) {
                eventos.botonNoCargando();
                return;
            }

            eventos.ocultarMsjErrores();

            var formData = new FormData(form[0]);

            funciones.Post($(form).prop("action"), formData, function (response) {
                if (response.isSuccessFully === true) {
                    eventos.botonCargando();
                    window.localStorage.clear();
                    window.location.href = siteLocation + 'NotificacionesCNBV';
                } else {
                    eventos.botonNoCargando();
                    eventos.mostrarMsjErrores(response.result);
                }
            });
        });

        $("#Usuario").focus(function () {
            eventos.ocultarMsjErrores();
        });

        $("#Contrasenia").focus(function () {
            eventos.ocultarMsjErrores();
        });

        eventos.ocultarMsjErrores();
    }

    var eventos = {

        ocultarMsjErrores: function () {
            $("#divAlerta").addClass("item-hide-Elemento");
        },
        mostrarMsjErrores: function (result) {
            $("#divAlerta").removeClass("item-hide-Elemento");
        },
        botonCargando: function () {

            document.querySelector('#btnSesion').setAttribute('data-kt-indicator', 'on');
            $("#btnSesion").prop('disabled', true);

        },
        botonNoCargando: function () {

            document.querySelector('#btnSesion').removeAttribute('data-kt-indicator', 'on');
            $("#btnSesion").prop('disabled', false);
        }
    }

    var funciones = {

        Post: function (url, data, callback) {
            $.ajax({
                url: url,
                type: 'POST',
                processData: false,
                contentType: false,
                data: data,
                success: callback
            });

        }

    }

    eventosControl();

})(jQuery);



