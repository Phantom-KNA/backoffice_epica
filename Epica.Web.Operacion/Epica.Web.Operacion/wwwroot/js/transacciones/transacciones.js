"use strict";
var datatable_myAccounts;

//Agregar la tabla de transacciones
var KTDatatableTransacciones = (function () {
    var table;

    var init = function () {
        datatable_myAccounts = $("#kt_datatable_movements").DataTable({
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
                url: "Transacciones/Consulta",
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
                { data: "fechaAlta", name: "Fecha", title: "FECHA" },
                {
                    data: "estatus", name: "Estatus", title: "ESTATUS",
                    render: function (data, type, row) {
                        if (data == 0) {
                            return "<span class='badge badge-light-info'>En Proceso</span>"
                        } else if ((data == 1) || (data == 2)) {
                            return "<span class='badge badge-light-warning'>En Tránsito</span>"
                        } else if (data == 4) {
                            return "<span class='badge badge-light-success'>Liquidada</span>"
                        } else if (data == 5) {
                            return "<span class='badge badge-light-danger'>Devuelto</span>"
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
                },
            ],
        });
        $('thead tr').addClass('text-start text-gray-400 fw-bold fs-7 text-uppercase gs-0');
    };

    var handleSearchDatatable = function () {
        var filterSearch = document.getElementById('search_input');
        filterSearch.addEventListener('keyup', function (e) {
            datatable_myAccounts.search(e.target.value).draw();
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
        datatable_myAccounts.ajax.reload();
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

//function modalCrearTransaccion() {
//    $('#newTransaccionModal').modal('show');
//};

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


