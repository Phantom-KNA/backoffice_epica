"use strict";

function validar2() {
    var user = document.getElementById("username").value;
    if (user.length <= 0) {
        document.getElementById("messageEmail").innerHTML = "Es necesario ingresar un usuario.";
        document.querySelector("#validacion").disabled = true;
    }
    else {
        document.getElementById("messageEmail").innerHTML = "";
        document.querySelector("#validacion").disabled = false;
    }
}

document.getElementById('kt_sign_in_form').addEventListener('keydown', function (event) {
    if (event.key === 13) {
        event.preventDefault();
        document.getElementById('validacion').click();
    }
});

document.getElementById('staticBackdrop').addEventListener('keydown', function (event) {
    if (event.key === 13) {
        event.preventDefault();
        document.getElementById('kt_sign_in_submit').click();
    }
});

var signin = function () {
    var form;
    var btnSubmit;

    var init = function () {
        form = document.querySelector("#kt_sign_in_form");
        btnSubmit = document.querySelector("#kt_sign_in_submit");

        var validator = FormValidation.formValidation(form, {
            fields: {
                text: {
                    validators: {
                        notEmpty: {
                            message: 'El usuario es requerido'
                        }
                    }
                },
                password: {
                    validators: {
                        notEmpty: {
                            message: 'La contraseña es requerida'
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap5({
                    rowSelector: '.fv-row',
                    eleInvalidClass: '',
                    eleValidClass: ''
                })
            }
        });

        btnSubmit.addEventListener('click', function (e) {
            e.preventDefault();
            if (validator) {
                validator.validate().then(function (status) {
                    if (status === 'Valid') {
                        btnSubmit.setAttribute('data-kt-indicator', 'on');
                        btnSubmit.disabled = true;
                        form.submit();
                    } else {
                        Swal.fire({
                            text: "Se han detectado algunos errores.",
                            icon: "error",
                            buttonsStyling: !1,
                            confirmButtonText: "Aceptar",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        });
                    }
                });
            }
        });

        // Obtén el elemento que contiene el mensaje de error
        var errorMessageElement = document.getElementById('error-message');

        // Si el elemento existe, obten el mensaje de error del atributo data-error
        if (errorMessageElement) {
            var errorMessage = errorMessageElement.getAttribute('data-error');

            // Verifica si errorMessage tiene un valor
            if (errorMessage && errorMessage.length > 0) {
                // Si errorMessage tiene un valor, muestra un alerta de SweetAlert2
                Swal.fire({
                    text: errorMessage, // Usa errorMessage como el texto del alerta
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Aceptar",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                });
            }
        }
    };

    return {
        init: function () {
            init();
        }
    }
}();

jQuery(document).ready(function () {
    document.querySelector("#validacion").disabled = true;
    signin.init();
});
