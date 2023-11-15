"use strict"
toastr.options.preventDuplicates = true;

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
            ordering: true,
            ajax: {
                url: siteLocation + 'Clientes/ConsultaEmpleados',
                type: 'POST',
                error: function (jqXHR, textStatus, errorThrown) {
                    $(".dataTables_processing").hide();
                    toastr.error("Se agoto el tiempo de espera para consultar estos datos. Inténtelo más tarde.");
                },
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
                    data: 'vinculo', name: 'NombreCompleto', title: 'Nombre Completo',
                    render: function (data, type, row) {
                        var partes = data.split("|"); 
                        var Nombre = partes[0];
                        var idCliente = partes[1];
                        return "<a href='/Clientes/Detalle/DatosGenerales?id=" + idCliente + "'>" + Nombre + "</a>";
                    }
                },
                { data: 'email', name: 'Email', title: 'Correo Electrónico' },
                { data: 'curp', name: 'Curp', title: 'CURP' },
                { data: 'puesto', name: 'Puesto', title: 'Puesto' },
                { data: 'area', name: 'Area', title: 'Area' },
                { data: 'roles', name: 'Roles', title: 'Roles' },
                {
                    data: 'active', name: 'active', title: 'Estatus',
                    render: function (data, type, row) {
                        if ((data == "5") || (data == "10")) {
                            return "<span class='badge badge-light-success' >Activo</span>";
                        } else if (data == "-10") {
                            return "<span class='badge badge-light-danger' >Bloqueo Total</span>";
                        }
                    }
                },
                {
                    data: 'estatusWeb', name: 'EstatusWeb', title: 'Estatus Web',
                    render: function (data, type, row) {
                        if (data == "1") {
                            return "<span class='badge badge-light-success' >Activo</span>";
                        } else if (data == "2") {
                            return "<span class='badge badge-light-danger' >Bloqueo Web</span>";
                        }
                    }
                }
            ],
        });
        $('thead tr').addClass('text-start text-gray-400 fw-bold fs-7 text-uppercase gs-0');

    };

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

    var ErrorControl = function () {
        dataTable.ext.errMode = 'throw';
    };

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
        },
        ErrorControl: function () {
            ErrorControl();
        },
    };
}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});

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