"use strict"

var datatable;

var KTDatatableRemoteAjax = function () {
    var table;
    var initDatatable = function () {
        datatable = $('#kt_datatable').DataTable({
            "order": [],
            pageLength: 15,
            language: {
                "decimal": "",
                "emptyTable": "No hay información",
                "info": "Mostrando _END_ de _TOTAL_ entradas",
                "infoEmpty": "Mostrando 0 de 0 entradas",
                "infoFiltered": "(Filtrado de _MAX_ total entradas)",
                "infoPostFix": "",
                "thousands": ",",
                "lengthMenu": "Mostrar _MENU_ Entradas",
                "loadingRecords": "Cargando...",
                "processing": "Procesando...",
                "search": "Buscar:",
                "zeroRecords": "No se encontraron resultados para mostrar",
                "paginate": {
                    "first": "Primero",
                    "last": "Último",
                    "next": "&raquo;",
                    "previous": "&laquo;"
                }
            },
            responsive: true,
            pagingType: 'simple_numbers',
            searching: true,
            lengthMenu: [15, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            ajax: {
                url: siteLocation + 'Clientes/ConsultaCuentas',
                type: 'POST',
                data: function (d) {
                    d.id = AccountId;
                },
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all",
            }],
            columns: [
                {
                    data: 'noCuenta', name: 'noCuenta', title: 'Número Cuenta',
                    render: function (data, type, row) {
                        var partes = data.split("|"); // Separar la parte entera y decimal
                        var NumCuenta = partes[0];
                        var idCuenta = partes[1];
                        var idCliente = partes[2];
                        return "<a href='/Clientes/Detalle/Movimientos?id=" + idCuenta + "&cliente=" + idCliente + "&noCuenta=" + NumCuenta + "'>" + NumCuenta + "</a>";
                    }
                },
                {
                    data: 'saldo', name: 'saldo', title: 'Saldo',
                    render: function (data, type, row) {
                        return accounting.formatMoney(data);
                    }
                },
                { data: 'alias', name: 'Alias', title: 'Alias' },
                { data: 'nombrePersona', name: 'NombrePersona', title: 'Nombre de Persona' },
                { data: 'fechaAltaFormat', name: 'fechaAltaFormat', title: 'Fecha de Alta' },
                { data: 'fechaActualizacionformat', name: 'fechaActualizacionformat', title: 'Fecha de Actualizacion' },
                {
                    data: 'estatus', name: 'Estatus', title: 'Estatus Cuenta',
                    render: function (data, type, row) {
                        return data == 1 ?
                            "<span class='badge badge-light-danger' >Desactivado</span>" : "<span class='badge badge-light-success' >Activo</span>";
                    }
                },
                {
                    data: 'bloqueoSPEIOut', name: 'BloqueoSpeiOut', title: 'Estatus Spei Out',
                    render: function (data, type, row) {
                        return data == 1 ?
                            "<span class='badge badge-light-danger' >Desactivado</span>" : "<span class='badge badge-light-success' >Activo</span>";
                    }
                },
                {
                    title: '',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            var htmlString = row.acciones;
                            return htmlString
                        }
                    }
                }
            ],
        });
        $('thead tr').addClass('text-start text-gray-400 fw-bold fs-7 text-uppercase gs-0');

    };

    $(document).on('click', '#GuardarCuenta', function (e) {

        

        toastr.success('Se guardó la información de manera exitosa', "");
        $("#btnCerrarCuenta").click();
    });

    var plugins = () => {
        $('.form-select2').select2();
    }

    //var exportButtons = () => {
    //    const documentTitle = 'Personas Fisicas';
    //    var buttons = new $.fn.dataTable.Buttons(table, {
    //        buttons: [
    //            {
    //                extend: 'excelHtml5',
    //                title: documentTitle,
    //                exportOptions: {
    //                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8]
    //                }
    //            },
    //            {
    //                extend: 'pdfHtml5',
    //                title: documentTitle,
    //                exportOptions: {
    //                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8]
    //                },
    //                orientation: 'landscape'
    //            }
    //        ]
    //    }).container().appendTo($('#kt_datatable_buttons'));

    //    const exportButtons = document.querySelectorAll('#kt_datatable_export_menu [data-kt-export]');
    //    exportButtons.forEach(exportButton => {
    //        exportButton.addEventListener('click', e => {
    //            e.preventDefault();

    //            const exportValue = e.target.getAttribute('data-kt-export');
    //            const target = document.querySelector('.dt-buttons .buttons-' + exportValue);
    //            target.click();
    //        });
    //    });
    //}

    var handleSearchDatatable = () => {
        /*const filterSearch = document.querySelector('[data-kt-filter="search"]');*/
        var filterSearch = document.getElementById('search_input');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    var recargar = function () {
        datatable.ajax.reload();
    }

    return {
        init: function () {
            //init();
            table = document.querySelector('#kt_datatable');

            if (!table) {
                return;
            }

            initDatatable();
            //exportButtons();
            plugins();
            handleSearchDatatable();
        },
        recargar: function () {
            recargar();
        }
    };
}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});


$(document).on('click', '.btnDetalle', function (e) {
    var id = $(this).data('id');
    ModalDetalle.init(id);
});

var ModalDetalle = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "Cuenta/DetalleCuenta",
            data: { 'numCuenta': id },
            success: function (result) {
                if (result.error) {
                    $(window).scrollTop(0);
                    $("#DivSuccessMessage").hide();
                    $("#DivErrorMessage").show();
                    setTimeout(function () { $("#DivErrorMessage").hide() }, 3000);
                    $("#ErrorMessage").text(result.errorDescription);
                } else {
                    $('#modal_detalle #modalLabelTitle').html('Detalle de Cuenta');
                    $('#modal_detalle .modal-body').html(result.result);
                    $('#modal_detalle').modal('show');
                }

                return;
            },
            error: function (res) {
                $("#DivSuccessMessage").hide();
                $("#DivErrorMessage").show();
                $("#ErrorMessage").text('Error');
            }
        });
        listeners();
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {

    }
    // Cerramos la ventana modal
    var cerrarModal = function () {
        $('#btnCancelar').click();
    }
    return {
        init: function (id) {
            init(id);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();

$(document).on('click', '.btnAsignacion', function (e) {
    ModalAsignacion.init();
});

var ModalAsignacion = function () {

    var init = function () {
        abrirModal();
    }
    var abrirModal = function () {
        $('#modal_asignacion #modalLabelTitle').html('Asignar Cuenta');
        $('#modal_asignacion').modal('show');
        listeners();
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {

    }
    // Cerramos la ventana modal
    var cerrarModal = function () {
        $('#btnCancelar').click();
    }
    return {
        init: function () {
            init();
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();

$(document).on('click', '#btnBuscarCliente', function () {

    const inputNombreCliente = document.getElementById('nombreCliente');
    const NoCuenta = inputNombreCliente.value;

    toastr.info("Localizando cuenta.");

    $.ajax({
        url: "/Clientes/BuscarCuenta",
        type: "POST",
        data: { NoCuenta: NoCuenta },
        success: function (result) {

            const idCuenta = document.getElementById('IdCuenta');
            const NumCuenta = document.getElementById('numeroCuenta');
            NumCuenta.value = result.descripcion;
            idCuenta.value = result.id;
            toastr.success("Cuenta localizada.");
        },
        error: function (error) {
            toastr.danger("No se pudo localizar la cuenta.");
        }
    });
});

$(document).on('click', '#GuardarAsignacion', function (e) {

    toastr.info('Almacenando Cuenta Asignada...', "");

    var form = $("#AsignacionForm")
    var valdata = form.serialize();

    $.ajax({
        url: "Cuentas/RegistrarAsignacionCuenta",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: valdata,
        success: function (data) {

            $('#modal_asignacion').modal('toggle');

            if (data.error == true) {
                Swal.fire(
                    'Asignar Cuenta',
                    'Hubo un problema al asignar la cuenta, verifique su existencia o Inténtalo más tarde.',
                    'error'
                )
            } else {
                datatable.ajax.reload();
                Swal.fire(
                    'Asignar Cuenta',
                    'Se guardó la información de manera exitosa.',
                    'success'
                )
            }
        },
        error: function (xhr, status, error) {
        }


    });
    $("#btnCerrarCuenta").click();
});

$(document).on('click', '.btnDesasignar', function (e) {

    Swal.fire({
        title: 'Desvincular Cuenta',
        text: "¿Estás seguro de que deseas desvincular esta cuenta del cliente?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {

            var idCliente = $(this).data('cliente');
            var idCuenta = $(this).data('id');

            $.ajax({
                url: "Cuentas/DesvincularCuenta",
                type: "POST",
                data: { idCuenta: idCuenta, idCliente: idCliente },
                success: function (data) {

                    if (data.error == true) {
                        Swal.fire(
                            'Desvincular Cuenta',
                            'Hubo un problema al desvincular la cuenta, verifique su existencia o Inténtalo más tarde.',
                            'error'
                        )
                    } else {
                        datatable.ajax.reload();
                        Swal.fire(
                            'Desvincular Cuenta',
                            'Se ha desvinculado la cuenta con éxito.',
                            'success'
                        )
                    }

                },
                error: function (xhr, status, error) {
                }

            });
        }
    })
});