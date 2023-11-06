toastr.options.preventDuplicates = true;

var table;
var datatable;
var filterAccount;

var KTDatatableRemoteAjax = function () {
    var table;
    var initDatatable = function () {
        datatable = $('#kt_datatable').DataTable({
            "order": [],
            pageLength: 10,
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
                url: siteLocation + 'Clientes/ConsultarListadoDocumentos',
                type: 'POST',
                error: function (jqXHR, textStatus, errorThrown) {
                    $(".dataTables_processing").hide();
                    toastr.error("Se agoto el tiempo de espera para consultar estos datos. Inténtelo más tarde.");
                },
                data: function (d) {
                    d.idAccount = AccountId;
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
                { data: 'descripcionDocumento', name: 'DescripcionDocumento', title: 'Documento' },
                { data: 'observaciones', name: 'Observaciones', title: 'Observaciones' },
                { data: 'fechaalta', name: 'fecha_alta', title: 'Fecha de Registro' },
                { data: 'fechaactualizacion', name: 'fecha_actualizacion', title: 'Fecha de Actialización' },
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

    var handleSearchDatatable = function () {
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
            plugins();
            handleSearchDatatable();
            //handleFilterDatatable();
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


var plugins = () => {
    $('#documento').filer({
        addMore: true,
        limit: 1,
        maxSize: 30,
        extensions: ["jpeg","jpg","png","pdf"],
        showThumbs: true,
        captions: {
            button: "Cargar Archivo",
            feedback: "Cargar Documento Cliente",
            feedback2: "Archivos Agregados",
            removeConfirmation: "¿Estás seguro que deseas eliminar este documento?",
            errors: {
                filesLimit: "Únicamente se pueden cargar {{fi-limit}} archivos.",
                filesType: "El formato del documento, no es válido.",
                filesSize: "{{fi-name}} supera el límite permitido! El tamaño permitido es de {{fi-maxSize}} MB."
            }
        }
    });

    //$('.form-select2').select2();
    //myDropzone = new Dropzone("#dropfile", {
    //    autoProcessQueue: false,
    //    url: "/",
    //    maxFiles: 1,
    //    maxFilesize: 5, // MB
    //    addRemoveLinks: true,
    //    uploadMultiple: false,
    //    acceptedFiles: ".jpeg,.jpg,.png,application/pdf",
    //    addedfiles: function (files) {
    //        if (files[0].accepted == false) {
    //            if (this.files.length >= 1)
    //                toastr.error("Elimine el documento antes de agregar uno nuevo.", "BAK")
    //            else
    //                toastr.error("Solo se permiten archivos de 5MB.", "BAK")
    //            this.removeFile(files[0])
    //        }
    //    },

    //});
}

$(document).on('click', '#BtnGuardarDocumento', function (e) {

    e.preventDefault();

    var tipoDocumento = $('#tipoDocumento').val();
    var documento = $('#documento').val();

    if (!tipoDocumento || !documento) {
        toastr.error('Por favor, complete todos los campos obligatorios.', "").preventDuplicates;
        return; // Detener la ejecución si los campos no están completos
    }

    toastr.info('Almacenando Documento...', "").preventDuplicates; 
    var form = new FormData($('#CargaDocumentoForm')[0]);

    $.ajax({
        url: "Documentos/CargarDocumentoCliente",
        type: "POST",
        dataType: 'json',
        contentType: false,
        processData: false,
        data: form,
        success: function (data) {

            datatable.ajax.reload();
            $('#kt_modal_Nuevo').modal('toggle');
            $('#CargaDocumentoForm')[0].reset();
            var filerKit = $("#documento").prop("jFiler");
            filerKit.reset();

            toastr.success('Se guardó la información de manera exitosa', "").preventDuplicates;
        },
        error: function (xhr, status, error) {
        }


    });
    $("#btnCerrarCuenta").click();
});

$('#kt_modal_Nuevo').on('hidden.bs.modal', function () {
    // Limpiar campos del modal al cerrarlo
    $('#tipoDocumento').val('');
    var filerKit = $("#documento").prop("jFiler");
    filerKit.reset();
});

$(document).on('click', '#btnVerDocumento', function (e) {

    var DocAllySend = $(this).data('id');

    toastr.info('Consultando Documento...', "");

    $.ajax({
        url: "Documentos/VerDocumentoCliente",
        type: "POST",
        dataType: 'json',
        data: { DocAlly: DocAllySend },
        success: function (data) {

            if (data.error == false) {

                var TipoVisualizador = "";

                if (data.codigo == "PDF") {
                    TipoVisualizador = "data:application/pdf;";
                } else {
                    TipoVisualizador = "data:image;";
                }

                var contenidoBase64 = TipoVisualizador + "base64," + data.Archivo64;

                // Verifica si el contenido es una imagen o PDF
                if (contenidoBase64.startsWith("data:image")) {
                    $("#Ver_Docto .modal-body").html('<img src="' + contenidoBase64 + '" class="img-fluid">');
                } else if (contenidoBase64.startsWith("data:application/pdf")) {
                    $("#Ver_Docto .modal-body").html('<embed src="' + contenidoBase64 + '" type="application/pdf" style="width:100%;height:500px;">');
                } else {
                    $("#Ver_Docto .modal-body").html('<p>Formato no admitido</p>');
                }
                $('#Ver_Docto').modal('show');

            } else {
                toastr.error('Hubo un problema al procesar el documento. Verifique su existencia.', "");
            }

        },
        error: function (xhr, status, error) {
        }


    });
});