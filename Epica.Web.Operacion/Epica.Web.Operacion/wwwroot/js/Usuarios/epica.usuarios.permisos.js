var table;
var datatable;
var filterAccount;

var KTDatatableRemoteAjax = function () {
    var table;
    var initDatatable = function () {
        datatable = $('#kt_datatable_user').DataTable({
            "order": [],
            pageLength: 15,
            language: {
                "decimal": "",
                "emptyTable": "No hay información disponible",
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
            lengthMenu: [5, 10, 15, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            ajax: {
                url: siteLocation + 'Usuarios/ConsultaPermisos',
                type: 'POST',
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
            map: function (raw) {
                // sample data mapping
                var dataSet = raw;
                if (typeof raw.data !== 'undefined') {
                    dataSet = raw.data;
                }
                return dataSet;
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'id', name: 'Id', title: 'Id Usuario' },
                { data: 'nombreRol', name: 'nombreRol', title: 'Nombre del Rol' },
                {
                    data: 'listaGen', name: 'Transacciones', title: 'Transacciones',
                    render: function (data, type, row) {

                        var renderList = "<ul>";

                        $(data).each(function (entry, value) {                           
                            if (value.vista == "Transacciones") {

                                if (value.escritura == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Crear Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Crear Registros</span></label></li>"
                                }

                                if (value.lectura == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Consultar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Consultar Registros</span></label></li>"
                                }

                                if (value.eliminar == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Eliminar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Eliminar Registros</span></label></li>"
                                }

                                if (value.actualizar == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Modificar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Modificar Registros</span></label></li>"
                                }
             
                            }                        
                        });

                        renderList += "</ul>";
                        return renderList;
                    }
                },
                {
                    data: 'listaGen', name: 'Clientes', title: 'Clientes',
                    render: function (data, type, row) {
                        var renderList = "<ul>";
                        $(data).each(function (entry, value) {
                            if (value.vista == "Clientes") {

                                if (value.escritura == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Crear Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Crear Registros</span></label></li>"
                                }

                                if (value.lectura == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Consultar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Consultar Registros</span></label></li>"
                                }

                                if (value.eliminar == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Eliminar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Eliminar Registros</span></label></li>"
                                }

                                if (value.actualizar == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Modificar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Modificar Registros</span></label></li>"
                                }

                            }
                        });

                        renderList += "</ul>";
                        return renderList;
                    }
                },
                {
                    data: 'listaGen', name: 'Tarjetas', title: 'Tarjetas',
                    render: function (data, type, row) {
                        var renderList = "<ul>";

                        $(data).each(function (entry, value) {
                            if (value.vista == "Tarjetas") {

                                if (value.escritura == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Crear Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Crear Registros</span></label></li>"
                                }

                                if (value.lectura == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Consultar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Consultar Registros</span></label></li>"
                                }

                                if (value.eliminar == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Eliminar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Eliminar Registros</span></label></li>"
                                }

                                if (value.actualizar == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Modificar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Modificar Registros</span></label></li>"
                                }

                            }
                        });

                        renderList += "</ul>";
                        return renderList;
                    }
                },
                {
                    data: 'listaGen', name: 'Cuentas', title: 'Cuentas',
                    render: function (data, type, row) {
                        var renderList = "<ul>";

                        $(data).each(function (entry, value) {
                            if (value.vista == "Tarjetas") {

                                if (value.escritura == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Crear Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Crear Registros</span></label></li>"
                                }

                                if (value.lectura == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Consultar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Consultar Registros</span></label></li>"
                                }

                                if (value.eliminar == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Eliminar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Eliminar Registros</span></label></li>"
                                }

                                if (value.actualizar == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Modificar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Modificar Registros</span></label></li>"
                                }

                            }
                        });

                        renderList += "</ul>";
                        return renderList;
                    }
                },
                {
                    data: 'listaGen', name: 'Configuracion', title: 'Configuracion',
                    render: function (data, type, row) {
                        var renderList = "<ul>";

                        $(data).each(function (entry, value) {
                            if (value.vista == "Configuracion") {

                                if (value.escritura == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Crear Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Crear Registros</span></label></li>"
                                }

                                if (value.lectura == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Consultar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Consultar Registros</span></label></li>"
                                }

                                if (value.eliminar == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Eliminar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Eliminar Registros</span></label></li>"
                                }

                                if (value.actualizar == true) {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'><input class='form-check-input' name='communication[]' type='checkbox' value='1' checked><span class='fw-semibold ps-2 fs-6'>Modificar Registros</span></label></li>"
                                } else {
                                    renderList += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-invalid'><input class='form-check-input' name='communication[]' type='checkbox' value='1'><span class='fw-semibold ps-2 fs-6'>Modificar Registros</span></label></li>"
                                }

                            }
                        });

                        renderList += "</ul>";
                        return renderList;
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

    var exportButtons = () => {
        const documentTitle = 'Usuarios';
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6, 7, 8]
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

    var handleSearchDatatable = function () {
        var filterSearch = document.getElementById('search_input');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Reset Filter
    var handleResetForm = () => {
        // Select reset button
        const resetButton = document.querySelector('[data-kt-customer-table-filter="reset"]');

        // Reset datatable
        resetButton.addEventListener('click', function () {
            datatable.search('').draw();
        });
    }

    var recargar = function () {
        datatable.ajax.reload();
    }

    $(".btn-filtrar").click(function () {
        recargar();
    })

    $(".btn_limpiar_filtros").click(function () {
        $(".filtro-control").val('');
        $(".filtro-control-select").val(null).trigger('change');;
        recargar();
    });

    return {
        init: function () {
            //init();
            table = document.querySelector('#kt_datatable_user');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
            //handleFilterDatatable();
        },
        recargar: function () {
            recargar();
        }
    };
}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});

function GestionarUsuario(AccountID, estatus) {
    $.ajax({
        url: siteLocation + 'Clientes/GestionarEstatusCliente',
        async: true,
        cache: false,
        type: 'POST',
        data: { id: AccountID, Estatus: estatus },
        success: function (data) {

            datatable.ajax.reload();
            alert("Se ha actualizado la cuenta con exito.");
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}