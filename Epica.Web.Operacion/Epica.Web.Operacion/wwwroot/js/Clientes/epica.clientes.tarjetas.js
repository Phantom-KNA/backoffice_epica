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
                url: siteLocation + 'Clientes/ConsultaTarjetas',
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
                { data: "nombreCompleto", name: "nombreCompleto", title: "Nombre de Titular" },
                {
                    data: "tarjeta", name: "tarjeta", title: "Número de Tarjeta",
                    render: function (data, type, row) {
                        return data.replace(/\d(?=\d{4})/g, "*");
                    }
                },
                { data: "clabe", name: "clabe", title: "Clabe" },
                { data: "proxyNumber", name: "proxyNumber", title: "Proxy" }
                //{
                //    title: '',
                //    orderable: false,
                //    data: null,
                //    defaultContent: '',
                //    render: function (data, type, row) {
                //        if (type === 'display') {
                //            var htmlString = row.acciones;
                //            return htmlString
                //        }
                //    }
                //}
            ],
        });
        $('thead tr').addClass('text-start text-gray-400 fw-bold fs-7 text-uppercase gs-0');

    };

    $(document).on('click', '#GuardarTarjetas', function (e) {

        toastr.info('Almacenando Tarjeta...', "");

        var form = $("#TarjetasForm")
        var valdata = form.serialize();
        
        $.ajax({
            url: "Tarjetas/RegistrarTarjetas",
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: valdata,
            success: function (data) {

                if (data.error == true) {
                    Swal.fire(
                        'Agregar Tarjeta Toka',
                        'Hubo un problema al agregar esta tarjeta, verifique su existencia o Inténtelo más tarde.',
                        'error'
                    )
                } else {
                    datatable.ajax.reload();
                    Swal.fire(
                        'Desvincular Cuenta',
                        'Se guardo la información de manera exitosa.',
                        'success'
                    )
                }
            },
            error: function (xhr, status, error) {
                console.log(error);
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
    //toastr.success("<br /><br /><button type='button' id='confirmationButtonYes' class='btn clear'>Yes</button><button type='button' id='confirmationButtonYes' class='btn clear'>NO</button>", 'delete item?',
    //    {
    //        closeButton: false,
    //        allowHtml: true,
    //        onShown: function (toast) {
    //            $("#confirmationButtonYes").click(function () {
    //                console.log('clicked yes');
    //            });
    //        }
    //    });
    KTDatatableRemoteAjax.init();
});