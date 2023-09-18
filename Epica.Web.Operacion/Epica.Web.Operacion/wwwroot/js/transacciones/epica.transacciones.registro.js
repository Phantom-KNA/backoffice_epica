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

        $(document).on('click', '#btnGuardarTransaccion', function (e) {
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