
"use strict";
var datatable_myAccounts;

//Agregar la tabla de transacciones
var KTDatatableTransacciones = (function () {
    var table;

    var init = function () {
        datatable_myAccounts = $("#kt_datatable_movements").DataTable({
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
                {
                    data: "vinculo", name: "vinculo", title: "CLAVE DE RASTREO",
                    render: function (data, type, row) {
                        var partes = data.split("|"); // Separar la parte entera y decimal
                        var ClaveRastreo = partes[0];
                        var idCliente = partes[1];

                        if (ClaveRastreo == "N/A") {
                            return ClaveRastreo;
                        } else {
                            return "<a href='/Clientes/Detalle/Movimientos?cliente=" + idCliente + "'>" + ClaveRastreo + "</a>";
                        }
                        
                    }
                },
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
                { data: "fechaInstruccion", name: "FechaInstruccion", title: "FECHA INSTRUCCIÓN" },
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
            //plugins();
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

$(document).on('click', '.btnDetalle', function (e) {
    var id = $(this).data('id');
    var estatus = $(this).data('estatus');
    var claveCobranza = $(this).data('clabecobranza');

    var parametros = id + "|" + claveCobranza + "|" + estatus;
    ModalDetalle.init(parametros);
});

$(document).on('click', '#GuardarDocumento', function (e) {

    Swal.fire({
        title: 'Cargar Masiva Transacciones',
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
                url: "Transacciones/CargarDocumentoMasivoTransacciones",
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

                    if (result.permiso == true && $('#modal_detalle .modal-footer a.btn-editar').length === 0) {
                        var ruta = "/Transacciones/Modificar?id=" + id;
                        $('#modal_detalle .modal-footer').append("<a href='" + ruta + "' class='btn btn-info btn-sm font-weight-bold btn-editar'><i class='bi bi-pencil'></i>&nbsp;Editar Transacción</a>");
                    }

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

//function Registro() {
//    Swal.fire({
//        title: '<span class="swal-title-custom"><b>Valida tu identidad</b></span>',
//        text: 'Por favor, ingrese su token y código de seguridad:',
//        html:
//            '<label for="swal-input1" class="swal-label"><b>Token:</b></label>' +
//            '<input id="swal-input1" class="swal2-input" style="font-size:14px;width:250px"  placeholder="Token">' +
//            '<div style="margin-top: 20px;"></div>' +
//            '<label for="swal-input2" class="swal-label"><b>Código de Seguridad:</b></label>' +
//            '<input id="swal-input2" class="swal2-input" style="font-size:14px;width:250px" type="password" placeholder="Código de Seguridad">',
//        showCancelButton: true,
//        confirmButtonColor: '#0493a8',
//        confirmButtonText: 'Aceptar',
//        cancelButtonText: 'Cancelar',
//        preConfirm: () => {
//            const swalInput1 = Swal.getPopup().querySelector('#swal-input1').value;
//            const swalInput2 = Swal.getPopup().querySelector('#swal-input2').value;
//            return [swalInput1, swalInput2];
//        }
//    }).then((result) => {
//        if (result.isConfirmed) {
//            const [tokenInput, codigoInput] = result.value;

//            // Realiza la validación del token y código de seguridad
//            $.ajax({
//                url: '/Autenticacion/ValidarTokenYCodigo',
//                async: true,
//                cache: false,
//                type: 'POST',
//                data: { token: tokenInput, codigo: codigoInput },
//                success: function (validationResult) {
//                    if (validationResult.mensaje === true) {
//                        // Redirige a la vista "transacciones/registro"
//                        window.location.href = '/transacciones/registro';
//                    } else {
//                        // Token o código de seguridad incorrectos, muestra un mensaje de error
//                        Swal.fire(
//                            'Error',
//                            'Token o código de seguridad incorrectos. Inténtalo de nuevo.',
//                            'error'
//                        );
//                    }
//                },
//                error: function () {
//                }
//            });
//        }
//    });
//}

$("#filtro_cuenta_ordenante, #filtro_claveRastreo, #filtro_nombreBeneficiario, #filtro_transaccion, #filtro_monto").on("keydown", function (e) {
    if (e.keyCode === 13) {
        e.preventDefault();
        $("#btnAplicarFiltros").click();
    }
});