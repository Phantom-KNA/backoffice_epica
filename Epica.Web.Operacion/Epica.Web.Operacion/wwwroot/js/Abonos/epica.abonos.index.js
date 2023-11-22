
"use strict";
var datatable_myAccounts;

//Agregar la tabla de transacciones
var KTDatatableTransacciones = (function () {
    var table;

    var init = function () {
        datatable_myAccounts = $("#kt_datatable_abono").DataTable({
            // Configuraciones de la tabla...
            "order": [],
            pageLength: 20,
            language: {
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
                "zeroRecords": "No se encontraron resultados para mostrar"
            },
            responsive: true,
            pagingType: 'simple_numbers',
            searching: true,
            lengthMenu: [10, 15, 20, 25, 50, 100],
            processing: false,
            serverSide: true,
            filter: true,
            ordering: false,
            // Configuración de la fuente de datos AJAX
            ajax: {
                url: "Autorizador/Consulta",
                type: "POST",
                error: function (jqXHR, textStatus, errorThrown) {
                    $(".dataTables_processing").hide();
                    toastr.error("Se agoto el tiempo de espera para consultar estos datos. Inténtelo más tarde.");
                },
                data: function (d) {
                    var filtros = [];
                    $(".filtro-control").each(function () {
                        if ($(this).data('filtrar') == undefined) return;
                        filtros.push({
                            key: $(this).data('filtrar'),
                            value: $(this).val()
                        });
                    });
                    d.filters = filtros;
                },
            },
            // Configuración de las columnas
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: "claveRastreo", name: "ClaveRastreo", title: "Clave de Rastreo" },
                { data: "monto", name: "Monto", title: "Monto",
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
                { data: "cuentaOrdenante", name: "CuentaOrdenante", title: "Cuenta Ordenante" },
                { data: "nombreOrdenante", name: "NombreOrdenante", title: "Nombre Ordenante" },
                {
                    data: "concepto",
                    name: "Concepto",
                    title: "Concepto",
                    render: function (data, type, row) {
                        return '<div class="multiline-text" style="max-width: 150px; white-space: normal; word-wrap: break-word;">' + data + '</div>';
                    }
                },       
                { data: "fecha", name: "fecha", title: "Fecha"},
                { data: "descripcionEstatusAutorizacion", name: "DescripcionEstatusAutorizacion", title: "Estatus Autorización" },
                { data: "descripcioEstatusTransaccion", name: "DescripcionEstatusTransaccion", title: "Motivo Rechazo" },
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
                },
            ],
        });
        $('thead tr').addClass('text-start text-gray-400 fw-bold fs-7 text-uppercase gs-0');
    };

    var handleSearchDatatable = function () {
        const filterSearch = document.getElementById('search_input');
        filterSearch.addEventListener('keyup', function (e) {
            if (e.key === 'Enter') {
                if (filterSearch.value.length >= 6 && filterSearch.value.length <= 40) {
                    datatable_myAccounts.search(filterSearch.value).draw();
                }
            } else if (filterSearch.value === '') {
                datatable_myAccounts.search(filterSearch.value).draw();
            }
        });
    }

    var exportButtons = () => {
        const documentTitle = 'Autorizador';
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6, 7]
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_buttons'));

        const exportButtons = document.querySelectorAll('#kt_datatable_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);
                target.click();
            });
        });
    }

    // Función para descarga del archivo
    //var Download = function (idTransaccion) {
    //    var url = "Home/Archivo?idTransaccion=" + idTransaccion;
    //    window.location.href = url;
    //};

    $(".btn-filtrar").click(function () {
        reload();
    })

    $(".btn_limpiar_filtros").click(function () {
        $(".filtro-control").val('');
        $(".filtro-control-select").val(null).trigger('change');;
        reload();
    });

    // Función de recarga
    var reload = function () {
        datatable_myAccounts.ajax.reload();
    };

    var ErrorControl = function () {
        dataTable.ext.errMode = 'throw';
    };


    return {
        init: function () {
            table = document.querySelector("#kt_datatable_abono");

            //$(document.body).on('click', '#download-btn', function () {
            //    var idTransaccion = $(this).data('idtransaccion');
            //    Download(idTransaccion);
            //});

            if (!table) {
                return;
            }
            init();
            //plugins();
            handleSearchDatatable();
            exportButtons();

        },
        reload: function () {
            reload();
        },
        ErrorControl: function () {
            ErrorControl();
        },
    };
})();
$(document).ready(function () {
    KTDatatableTransacciones.init();
});

$("#filtro_cuenta_ordenante, #filtro_claveRastreo, #filtro_nombreBeneficiario, #filtro_transaccion, #filtro_monto").on("keydown", function (e) {
    if (e.keyCode === 13) {
        e.preventDefault();
        $("#btnAplicarFiltros").click();
    }
});

function AcreditarAbono(id) {
    Swal.fire({
        title: 'Acreditar Abono',
        text: "¿Estás seguro de que deseas acreditar el abono?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: siteLocation + 'Autorizador/AcreditarAbono',
                async: true,
                cache: false,
                type: 'PATCH',
                data: { id: id },
                success: function (response) {
                    console.log(response);
                    if (response.success === true) {
                        datatable_myAccounts.ajax.reload();
                        Swal.fire(
                            'Acreditación Abono',
                            response.message,
                            'success'
                        );
                    }
                    else {
                        datatable_myAccounts.ajax.reload();
                        Swal.fire(
                            'Acreditación Abono',
                            response.message,
                            'error'
                        );
                    }
                },
                error: function (xhr, status, error) {
                    datatable_myAccounts.ajax.reload();
                    Swal.fire(
                        'Acreditación Abono',
                        'Hubo un problema al realizar esta solicitud. Inténtalo más tarde.',
                        'danger'
                    );
                }
            });
        }
    });
}

function RechazarAbono(id) {
    Swal.fire({
        title: 'Rechazar Abono',
        text: "¿Estás seguro de que deseas rechazar el abono?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: siteLocation + 'Autorizador/RechazarAbono',
                async: true,
                cache: false,
                type: 'PATCH',
                data: { id: id },
                success: function (response) {
                    if (response.success === true) {
                        datatable_myAccounts.ajax.reload();
                        Swal.fire(
                            'Rechazar Abono',
                            response.message,
                            'success'
                        );
                    }
                    else {
                        datatable_myAccounts.ajax.reload();
                        Swal.fire(
                            'Rechazar Abono',
                            response.message,
                            'error'
                        );
                    }
                    reload();
                },
                error: function (xhr, status, error) {
                    datatable_myAccounts.ajax.reload();
                    Swal.fire(
                        'Rechazar Abono',
                        'Hubo un problema al realizar esta solicitud. Inténtalo más tarde.',
                        'danger'
                    );
                }
            });
        }
    });
}

$('#kt_datatable_abono').on('processing.dt', function (e, settings, processing) {
    if (processing) {
        KTApp.showPageLoading();
    } else {
        KTApp.hidePageLoading();
    }
})