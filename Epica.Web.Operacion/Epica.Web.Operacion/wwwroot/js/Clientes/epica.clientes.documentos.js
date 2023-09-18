var table;
var datatable;
var filterAccount;

var KTDatatableRemoteAjax = function () {
    var table;
    var initDatatable = function () {
        datatable = $('#kt_datatable').DataTable({
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
                url: siteLocation + 'Clientes/ConsultarListadoDocumentos',
                type: 'POST',
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
                { data: 'numeroIdentificacion', name: 'numeroIdentificacion', title: 'Numero de Identificación' },
                { data: 'nombreDocumento', name: 'NombreDocumento', title: 'Nombre del Documento' },
                { data: 'fechaUsuarioAlta', name: 'FechaUsuarioAlta', title: 'Fecha de Registro' },
                { data: 'fechaUsuarioActualizacion', name: 'FechaUsuarioActualizacion', title: 'Fecha de Actialización' },
                {
                    title: '',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        console.log(data);
                        if (type === 'display') {
                            var htmlString = row.acciones;
                            console.log(htmlString);
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
        }
    };
}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});


var plugins = () => {
    $('.form-select2').select2();
    myDropzone = new Dropzone("#dropfile", {
        autoProcessQueue: false,
        url: "/",
        maxFiles: 1,
        maxFilesize: 5, // MB
        addRemoveLinks: true,
        uploadMultiple: false,
        acceptedFiles: ".jpeg,.jpg,.png,application/pdf",
        addedfiles: function (files) {
            if (files[0].accepted == false) {
                if (this.files.length >= 1)
                    toastr.error("Elimine el documento antes de agregar uno nuevo.", "BAK")
                else
                    toastr.error("Solo se permiten archivos de 5MB.", "BAK")
                this.removeFile(files[0])
            }
        },

    });
}

$(document).on('click', '#BtnGuardarDocumento', function (e) {

    toastr.info('Almacenando Documento...', ""); 
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
            toastr.success('Se guardo la informacion de manera exitosa', "");
        },
        error: function (xhr, status, error) {
            console.log(error);
        }


    });
    $("#btnCerrarCuenta").click();
});