var table;
var datatable;
var filterAccount;
toastr.options.preventDuplicates = true;

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
            processing: false,
            serverSide: true,
            filter: true,
            ordering: false,
            ajax: {
                url: siteLocation + 'Usuarios/Consulta',
                type: 'POST',
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
                { data: 'username', name: 'Username', title: 'Nombre de Usuario' },
                { data: 'email', name: 'email', title: 'Correo Electrónico' },
                { data: 'descripcionRol', name: 'DescripcionRol', title: 'Rol' },
                { data: 'fechaAlta', name: 'FechaAlta', title: 'Fecha Alta' },
                { data: 'fechaUltimoAcceso', name: 'FechaUltimoAcceso', title: 'Fecha Último Acceso' },
                {
                    data: 'estatus', name: 'Estatus', title: 'Estatus',
                    render: function (data, type, row) {
                        return data == 1 ?
                            "<span class='badge badge-light-danger' >Desactivado</span>" : "<span class='badge badge-light-success' >Activo</span>";
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
                        columns: [0, 1, 2, 3, 4]
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
            if (e.key === 'Enter') {
                if (filterSearch.value.length >= 3 && filterSearch.value.length <= 40) {
                    datatable.search(e.target.value).draw();
                }
            } else if (filterSearch.value === '') {
                datatable.search(e.target.value).draw();
            }
        });
    }

    //var handleFilterDatatable = () => {
    //    // Select filter options
    //    filterAccount = document.querySelectorAll('[data-kt-customer-table-filter="payment_type"] [name="payment_type"]');
    //    const filterButton = document.querySelector('[data-kt-customer-table-filter="filter"]');

    //    // Filter datatable on submit
    //    filterButton.addEventListener('click', function () {
    //        // Get filter values
    //        //let paymentValue = '';

    //        //// Get payment value
    //        //filterAccount.forEach(r => {
    //        //    if (r.checked) {
    //        //        paymentValue = r.value;
    //        //    }

    //        //    // Reset payment value if "All" is selected
    //        //    if (paymentValue === 'all') {
    //        //        paymentValue = '';
    //        //    }
    //        //});

    //        // Filter datatable --- official docs reference: https://datatables.net/reference/api/search()
    //        datatable.search('4652364789632453').draw();
    //    });
    //}

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

    var ErrorControl = function () {
        dataTable.ext.errMode = 'throw';
    };

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
            table = document.querySelector('#kt_datatable');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
            //resetSubtable();
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

$(document).on('click', '#btnBuscarUsuario', function () {
    const inputNombreUsuario = document.getElementById('findnombreUsuario');
    const usuario = inputNombreUsuario.value;

    if ((usuario == null) || (usuario == "")) {
        toastr.warning("El valor ingresado, no es válido");
        return false;
    }

    toastr.info("Localizando cuenta.").preventDuplicates;
    toastr.clear();

    $.ajax({
        url: "Usuarios/BuscarUsuario",
        type: "POST",
        data: { usuario: usuario },
        success: function (result) {

            $("#idUsuario").empty();

            if (result.length == 0) {
                toastr.success("No se pudo localizar resultados para esta busqueda.").preventDuplicates;
            } else {
                $.each(result, function (index, value) {
                    $("#idUsuario").append('<option value="' + value.id + '">' + value.descripcion + '</option>');
                });
                toastr.success("Usuario localizado.").preventDuplicates;
            }
        },
        error: function (error) {
            toastr.warning("No se pudo localizar al usuario.").preventDuplicates;
        }
    });
});

$("#findnombreUsuario").on("keydown", function (e) {
    if (e.keyCode === 13) { 
        e.preventDefault();

        $("#btnBuscarUsuario").click();
    }
});

$(document).on('click', '#GuardarAsignacion', function (e) {
    e.preventDefault();

    var idUsuario = $('#idUsuario').val();
    var idRol = $('#idRol').val();

    if (!idUsuario || !idRol) {
        toastr.error('Por favor, complete todos los campos obligatorios.', "").preventDuplicates;
        return; // Detener la ejecución si los campos no están completos
    }


    toastr.info('Almacenando Rol Asignado...', "").preventDuplicates;

    var form = $("#AsignacionForm")
    var valdata = form.serialize();
    var cierre = 'true';

    $.ajax({
        url: "Usuarios/RegistrarAsignacionUsuario",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: valdata,
        success: function (data) {
            if (data.detalle == 'El usuario ya tiene un rol asignado.') {
                //$('#modal_asignacion').modal('toggle');
                //datatable.ajax.reload();
                toastr.warning('El usuario ya tiene un rol asignado, desvincule el Rol del Usuario', "").preventDuplicates;
                cierre = 'false';
            }
            else if (data.error == true) {
                $('#modal_asignacion').modal('toggle');
                datatable.ajax.reload();
                toastr.warning('Error al asignar el rol', "").preventDuplicates;
            } else {
                $('#modal_asignacion').modal('toggle');
                datatable.ajax.reload();
                toastr.success('Se guardó la información de manera exitosa', "").preventDuplicates;
            }

            if (cierre == 'true') {
                $('#idUsuario').val('');
                $('#nombreUsuario').val('');
                $('#findnombreUsuario').val('');
            }

        },
        error: function (xhr, status, error) {
        }


    });
    if (cierre == 'true') {
        $("#btnCerrarCuenta").click();
    }
});

$('#modal_asignacion').on('hidden.bs.modal', function () {
    // Limpiar campos del modal al cerrarlo
    $('#idUsuario').val('');
    $('#idRol').val('');
    $('#findnombreUsuario').val('');
});

function DesAsignarRol(idUser) {

    Swal.fire({
        title: 'Desasignar Rol a Usuario',
        text: "¿Estás seguro que deseas desasignar este rol?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: 'Usuarios/DesasignarUsuario',
                async: true,
                cache: false,
                type: 'POST',
                data: { idUsuario: idUser },
                success: function (data) {
                    if (data.error == true) {
                        Swal.fire(
                            'Error',
                            'Hubo un problema al gestionar los permisos de este usuario. Intentelo más tarde.',
                            'error'
                        );
                    } else {
                        datatable.ajax.reload();
                        Swal.fire(
                            'Usuario Actualizado',
                            'Se ha actualizado el usuario con éxito.',
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
$("#filtro_username, #filtro_rol, #filtro_fecha_alta, #filtro_fecha_acceso").on("keydown", function (e) {
    if (e.keyCode === 13) { 
        e.preventDefault();
        $("#btnAplicarFiltros").click();
    }
});




