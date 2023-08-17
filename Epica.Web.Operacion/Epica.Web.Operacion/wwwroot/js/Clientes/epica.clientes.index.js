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
                url: siteLocation + 'Clientes/Consulta',
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
                //{ data: 'id', name: 'Id', title: 'Id Usuario' },
                {
                    data: 'nombreCompleto', name: 'NombreCompleto', title: 'Nombre del Cliente',
                    render: function (data, type, row) {
                        var partes = data.split("|"); // Separar la parte entera y decimal
                        var Nombre = partes[0];
                        var ID = partes[1]
                        return "<a href='/Clientes/Detalle/DatosGenerales?id="+ID+"'>" + Nombre +"</a>";
                    }
                },
                { data: 'telefono', name: 'Telefono', title: 'Telefono' },
                { data: 'email', name: 'Email', title: 'Correo Electrónico' },
                { data: 'curp', name: 'Curp', title: 'CURP' },
                { data: 'organizacion', name: 'Organizacion', title: 'Organizacion' },
                //{ data: 'membresia', name: 'membresia', title: 'Tipo Membresia' },
                //{
                //    data: 'sexo', name: 'Sexo', title: 'Sexo',
                //    render: function (data, type, row) {
                //        return data == "M" ?
                //            "Masculino" : "Femenino";
                //    }
                //},
                {
                    data: 'estatus', name: 'Estatus', title: 'Estatus',
                    render: function (data, type, row) {
                        return data == 1 ?
                            "<span class='badge badge-light-success' >Activo</span>" : "<span class='badge badge-light-danger' >Desactivado</span>";
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
        const documentTitle = 'Clientes';
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5]
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

function GestionarClienteWeb(AccountID, estatus) {
    $.ajax({
        url: siteLocation + 'Clientes/GestionarEstatusClienteWeb',
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

function GestionarClienteTotal(AccountID, estatus) {
    $.ajax({
        url: siteLocation + 'Clientes/GestionarEstatusClienteTotal',
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