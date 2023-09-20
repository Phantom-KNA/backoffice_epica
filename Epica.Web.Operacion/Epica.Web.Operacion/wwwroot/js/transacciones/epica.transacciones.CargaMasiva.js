"use strict";
var datatable_transaccion;

//Agregar la tabla de transacciones
var KTDatatableTransacciones = (function () {
    var table;

    var init = function () {
        datatable_transaccion = $("#kt_datatable_movements").DataTable({
            // Configuraciones de la tabla...
            "order": [],
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
            lengthMenu: [10, 15, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            // Configuración de la fuente de datos AJAX
            ajax: {
                url: "ConsultaCargaMasiva",
                type: "POST",
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
                { data: "claveRastreo", name: "claveRastreo", title: "Clave de Rastreo" },
                { data: "ordenante", name: "Ordenante", title: "Cuenta Ordenante" },
                { data: "cuentaBeneficiario", name: "CuentaBeneficiario", title: "Cuenta Beneficiario" },
                {
                    data: "monto", name: "Monto", title: "Monto",
                    render: function (data, type, row) {
                        return accounting.formatMoney(data);
                    }
                },
                { data: "conceptoPago", name: "ConceptoPago", title: "Concepto" },
                { data: "fechaOperacion", name: "FechaOperacion", title: "Fecha Operación" },
                { data: "descripcionOperacion", name: "TipoOperacion", title: "Tipo de Operación" },
                { data: "descripcionMedioPago", name: "MedioPago", title: "Medio de Pago" },
                { data: "observaciones", name: "observaciones", title: "Observaciones" },
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
        var filterSearch = document.getElementById('search_input');
        filterSearch.addEventListener('keyup', function (e) {
            datatable_transaccion.search(e.target.value).draw();
        });
    }

    var exportButtons = () => {
        const documentTitle = 'Transacciones';
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
        datatable_transaccion.ajax.reload();
    };

    return {
        init: function () {
            table = document.querySelector("#kt_datatable_movements");

            //$(document.body).on('click', '#download-btn', function () {
            //    var idTransaccion = $(this).data('idtransaccion');
            //    Download(idTransaccion);
            //});

            if (!table) {
                return;
            }
            init();
            plugins();
            handleSearchDatatable();
            exportButtons();

        },
        reload: function () {
            reload();
        },
    };
})();
$(document).ready(function () {
    KTDatatableTransacciones.init();
});

var plugins = () => {

    $('#documento').filer({
        addMore: true,
        limit: 1,
        maxSize: 30,
        extensions: ["xlsx"],
        showThumbs: true,
        captions: {
            button: "Cargar Archivo",
            feedback: "Cargar Documento Excel",
            feedback2: "Archivos Agregados",
            removeConfirmation: "¿Estás seguro que deseas eliminar este documento?",
            errors: {
                filesLimit: "Únicamente se pueden cargar {{fi-limit}} archivos.",
                filesType: "El formato del documento no es válido.",
                filesSize: "{{fi-name}} supera el límite permitido! El tamaño permitido es de {{fi-maxSize}} MB."
            }
        }
    });
}

$(document).on('click', '#GuardarDocumento', function (e) {

    Swal.fire({
        title: 'Carga Masiva Transacciones',
        text: "¿Estás seguro de que deseas cargar este documento?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {

            toastr.info('Cargando Documento...', "");
            var form = new FormData($('#CargaDocumentoForm')[0]);

            $.ajax({
                url: "CargarDocumentoMasivoTransacciones",
                type: "POST",
                dataType: 'json',
                contentType: false,
                processData: false,
                data: form,
                success: function (data) {

                    if (data.error == true) {
                        Swal.fire(
                            'Carga Masiva Transacciones',
                            'Hubo un problema al cargar las transacciones, verifique que el documento sea válido.',
                            'error'
                        )
                    } else {
                        datatable_transaccion.ajax.reload();
                        Swal.fire(
                            'Carga Masiva Transacciones',
                            'Se han cargado las transacciones de forma exitosa.',
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

function EliminarTransaccionBatch(idRegistro) {

    Swal.fire({
        title: 'Eliminar Transacción',
        text: "¿Estás seguro que desea eliminar esta transacción?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: 'EliminarTransaccionBatch',
                async: true,
                cache: false,
                type: 'POST',
                data: { idRegistrio: idRegistro },
                success: function (data) {

                    if (data.error == true) {
                        Swal.fire(
                            'Eliminar Transacción',
                            'Hubo un problema al eliminar esta transacción, inténtelo más tarde o verifique su existencia.',
                            'error'
                        )
                    } else {
                        datatable_transaccion.ajax.reload();
                        Swal.fire(
                            'Eliminar Transacción',
                            'Se ha eliminado la transacción con éxito.',
                            'success'
                        )
                    }
                },
                error: function (xhr, status, error) {
                }
            });
        }
    })
}

function EditarTransaccionBatch(idRegistro) {

    $.ajax({
        cache: false,
        type: 'GET',
        url: "EditarTransaccionBatch",
        data: { 'idRegistro': idRegistro },
        success: function (result) {
            if (result.error) {
                $(window).scrollTop(0);
                $("#DivSuccessMessage").hide();
                $("#DivErrorMessage").show();
                setTimeout(function () { $("#DivErrorMessage").hide() }, 3000);
                $("#ErrorMessage").text(result.errorDescription);
            } else {
                $('#EditarTransaccion #modalLabelTitle').html('Editar Transacción');
                $('#EditarTransaccion .modal-body').html(result.result);

                $('#EditarTransaccion .modal-footer').html("<button type='button' class='btn btn-success btn-sm font-weight-bold' data-bs-dismiss='modal' aria-label='Close' id='btnCerrar'>Cerrar</button> <button type='button' class='btn btn-info btn-sm font-weight-bold' data-bs-dismiss='modal' aria-label='Close' id='btnGuardarModificaciones'>Guardar Modificaciones</button>");
                $('#EditarTransaccion').modal('show');
            }

            return;
        },
        error: function (res) {
            $("#DivSuccessMessage").hide();
            $("#DivErrorMessage").show();
            $("#ErrorMessage").text('Error');
        }
    });
}

$(document).on('click', '#btnGuardarModificaciones', function (e) {

    Swal.fire({
        title: 'Almacenar Cambios Transacción',
        text: "¿Estás seguro de que deseas almacenar estos cambios?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {

            toastr.info('Modificando Transacción...', "");
            var form = new FormData($('#EditTransaccionBatchForm')[0]);

            $.ajax({
                url: "EditarTransaccionBatch",
                type: "POST",
                dataType: 'json',
                contentType: false,
                processData: false,
                data: form,
                success: function (data) {

                    if (data.error == true) {
                        Swal.fire(
                            'Carga Masiva Transacciones',
                            'Hubo un problema al cargar las transacciones, verifique que el documento sea válido.',
                            'error'
                        )
                    } else {
                        datatable_transaccion.ajax.reload();
                        Swal.fire(
                            'Carga Masiva Transacciones',
                            'Se han cargado las transacciones de forma exitosa.',
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

$(document).on('click', '#btnAlmacenarTransacciones', function (e) {

    Swal.fire({
        title: 'Almacenar Cambios Transacción',
        text: "¿Estás seguro de que deseas almacenar estos cambios?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {
            var infoTabla = datatable_transaccion.page.info();

            var numPagina = infoTabla.start / infoTabla.length + 1; // calcular el número de página
            var Size = infoTabla.length; // número de registros por página

            $.ajax({
                url: "ProcesarTransaccionesMasiva",
                type: "POST",
                dataType: 'json',
                data: { 'pagina': numPagina, 'registros': Size },
                success: function (data) {

                    if (data.error == true) {
                        Swal.fire(
                            'Cargar Masiva Transacciones',
                            'Hubo un problema al cargar las transacciones, verifique que el documento sea válido.',
                            'error'
                        )
                    } else {
                        datatable_transaccion.ajax.reload();
                        Swal.fire(
                            'Carga Masiva Transacciones',
                            'Se han cargado las transacciones de forma exitosa.',
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
