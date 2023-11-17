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
            processing: false,
            serverSide: true,
            filter: true,
            ordering: false,
            ajax: {
                url: siteLocation + 'Cuenta/ConsultaCuentas',
                type: 'POST',
                error: function (jqXHR, textStatus, errorThrown) {
                    toastr.error("Se agoto el tiempo de espera para consultar estos datos. Inténtelo más tarde.");
                },
                data: function (d) {
                    d.id = AccountId,
                    d.TipoConsulta = TipoConsulta;
                },
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all",
            }],
            columns: [
                { data: "cuetaOrigenOrdenante", name: "cuetaOrigenOrdenante", title: "CUENTA ORDENANTE" },
                { data: "claveRastreo", name: "ClaveRastreo", title: "CLAVE DE RASTREO" },
                { data: "nombreOrdenante", name: "NombreCuenta", title: "NOMBRE ORDENANTE" },
                { data: "nombreBeneficiario", name: "Institucion", title: "NOMBRE BENEFICIARIO" },
                { data: "concepto", name: "Concepto", title: "CONCEPTO" },
                {
                    data: "monto",
                    name: "Monto",
                    title: "MONTO",
                    render: function (data, type, row) {
                        var color = 'black';//Color por defecto
                        if (row.monto > 0) {
                            color = 'green';//Color verde si es mayor a 0 
                        } else if (row.monto < 0) {
                            color = 'red';//Color rojo si es menor a 0
                        }
                        return '<pan style ="color: ' + color + '">' + accounting.formatMoney(data) + '<s/span>';
                    }
                },
                //{ data: "fechaAlta", name: "Fecha", title: "FECHA" },
                { data: "fechaAlta", name: "FechaAlta", title: "FECHA ALTA" },
                { data: "fechaAutorizacion", name: "FechaAutorizacion", title: "FECHA AUTORIZACIÓN" },
                {
                    data: "estatus", name: "Estatus", title: "ESTATUS",
                    render: function (data, type, row) {

                        if (data == 0) {
                            return "<span class='badge badge-light-info'>En Proceso</span>"
                        } else if ((data == 1) || (data == 2)) {
                            return "<span class='badge badge-light-warning'>En Tránsito</span>"
                        } else if (data == 4) {
                            return "<span class='badge badge-light-success'>Liquidada</span>"
                        }
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

        toastr.info('Almacenando Transacción...', "");

        var form = $("#TransaccionForm")
        var valdata = form.serialize();
        
        $.ajax({
            url: "Transacciones/RegistrarTransaccion",
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: valdata,
            success: function (data) {

                datatable.ajax.reload();
                toastr.success('Se guardó la información de manera exitosa', "");
            },
            error: function (xhr, status, error) {
            }


        });   
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

    //var ErrorControl = function () {
    //    dataTable.ext.errMode = 'throw';
    //}

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
        //ErrorControl: function () {
        //    ErrorControl();
        //}
    };
}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});


$(document).on('click', '.btnDetalle', function (e) {
    var id = $(this).data('id');
    var estatus = $(this).data('estatus');
    var claveCobranza = $(this).data('clabecobranza');

    var parametros = id + "|" + claveCobranza + "|" + estatus;
    ModalDetalle.init(parametros);
});



var ModalDetalle = function () {

    var init = function (parametros) {
        abrirModal(parametros);
    }
    var abrirModal = function (parametros) {
        var partes = parametros.split("|");
        var id = partes[0];
        var ClaveCobranza = partes[1];
        var Estatus = partes[2];

        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "Transacciones/DetalleTransaccion",
            data: { 'Id': id, 'Estatus': Estatus, 'ClabeCobranza': ClaveCobranza },
            success: function (result) {
                if (result.error) {
                    $(window).scrollTop(0);
                    $("#DivSuccessMessage").hide();
                    $("#DivErrorMessage").show();
                    setTimeout(function () { $("#DivErrorMessage").hide() }, 3000);
                    $("#ErrorMessage").text(result.errorDescription);
                } else {
                    $('#modal_detalle #modalLabelTitle').html('Detalle de Transacción');
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

$(document).on("click", "#Redireccion", function () {
    KTApp.showPageLoading();
});

$('#kt_datatable').on('processing.dt', function (e, settings, processing) {
    if (processing) {
        KTApp.showPageLoading();
    } else {
        KTApp.hidePageLoading();
    }
})