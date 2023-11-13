toastr.options.preventDuplicates = true;

var table;
var datatable;
var filterAccount;
var Arreglo;

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
                    //Arreglo = [];
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
                { data: 'nombreRol', name: 'nombreRol', title: 'Nombre del Rol' },
                {
                    data: 'listaGen', name: 'Transacciones', title: 'Transacciones',
                    render: function (data, type, row) {

                        var renderList = "<ul>";
                        Arreglo = data;
                        $(data).each(function (entry, value) {                           
                            if (value.vista == "Transacciones") {

                                if (value.escritura == true) {
                                    renderList += CrearEstructuraCheck(true, "Transacciones", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Transacciones", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                }

                                if (value.lectura == true) {
                                    renderList += CrearEstructuraCheck(true, "Transacciones", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Transacciones", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                }

                                if (value.eliminar == true) {
                                    renderList += CrearEstructuraCheck(true, "Transacciones", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Transacciones", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                }

                                if (value.actualizar == true) {
                                    renderList += CrearEstructuraCheck(true, "Transacciones", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Transacciones", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
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
                                    renderList += CrearEstructuraCheck(true, "Clientes", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Clientes", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                }

                                if (value.lectura == true) {
                                    renderList += CrearEstructuraCheck(true, "Clientes", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Clientes", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                }

                                if (value.eliminar == true) {
                                    renderList += CrearEstructuraCheck(true, "Clientes", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Clientes", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                }

                                if (value.actualizar == true) {
                                    renderList += CrearEstructuraCheck(true, "Clientes", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Clientes", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
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
                                    renderList += CrearEstructuraCheck(true, "Tarjetas", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Tarjetas", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                }

                                if (value.lectura == true) {
                                    renderList += CrearEstructuraCheck(true, "Tarjetas", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Tarjetas", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                }

                                if (value.eliminar == true) {
                                    renderList += CrearEstructuraCheck(true, "Tarjetas", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Tarjetas", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                }

                                if (value.actualizar == true) {
                                    renderList += CrearEstructuraCheck(true, "Tarjetas", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Tarjetas", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
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
                            if (value.vista == "Cuentas") {

                                if (value.escritura == true) {
                                    renderList += CrearEstructuraCheck(true, "Cuentas", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Cuentas", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                }

                                if (value.lectura == true) {
                                    renderList += CrearEstructuraCheck(true, "Cuentas", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Cuentas", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                }

                                if (value.eliminar == true) {
                                    renderList += CrearEstructuraCheck(true, "Cuentas", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Cuentas", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                }

                                if (value.actualizar == true) {
                                    renderList += CrearEstructuraCheck(true, "Cuentas", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Cuentas", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
                                }

                            }
                        });

                        renderList += "</ul>";
                        return renderList;
                    }
                },
                {
                    data: 'listaGen', name: 'Operaciones', title: 'Operaciones',
                    render: function (data, type, row) {
                        var renderList = "<ul>";

                        $(data).each(function (entry, value) {
                            if (value.vista == "Operaciones") {

                                if (value.escritura == true) {
                                    renderList += CrearEstructuraCheck(true, "Operaciones", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Operaciones", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                }

                                if (value.lectura == true) {
                                    renderList += CrearEstructuraCheck(true, "Operaciones", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Operaciones", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                }

                                if (value.eliminar == true) {
                                    renderList += CrearEstructuraCheck(true, "Operaciones", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Operaciones", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                }

                                if (value.actualizar == true) {
                                    renderList += CrearEstructuraCheck(true, "Operaciones", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Operaciones", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
                                }

                            }
                        });

                        renderList += "</ul>";
                        return renderList;
                    }
                },
                {
                    data: 'listaGen', name: 'Configuracion', title: 'Configuración',
                    render: function (data, type, row) {
                        var renderList = "<ul>";

                        $(data).each(function (entry, value) {
                            if (value.vista == "Configuracion") {

                                if (value.escritura == true) {
                                    renderList += CrearEstructuraCheck(true, "Configuracion", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Configuracion", value.idPermiso, value.idRol, "Escritura", "Crear Registros");
                                }

                                if (value.lectura == true) {
                                    renderList += CrearEstructuraCheck(true, "Configuracion", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Configuracion", value.idPermiso, value.idRol, "Lectura", "Consultar Registros");
                                }

                                if (value.eliminar == true) {
                                    renderList += CrearEstructuraCheck(true, "Configuracion", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Configuracion", value.idPermiso, value.idRol, "Eliminar", "Eliminar Registros");
                                }

                                if (value.actualizar == true) {
                                    renderList += CrearEstructuraCheck(true, "Configuracion", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
                                } else {
                                    renderList += CrearEstructuraCheck(false, "Configuracion", value.idPermiso, value.idRol, "Actualizar", "Modificar Registros");
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
            Swal.fire(
                'Gestionar Usuario',
                'Se ha actualizado la cuenta con éxito.',
                'success'
            )
        },
        error: function (xhr, status, error) {
        }
    });
}

function GestionarPermisos(event) {
    var isChecked = $(event).is(':checked');
    Swal.fire({
        title: '¿Deseas ' + (isChecked ? 'asignar' : 'quitar') + ' esta acción al módulo?',
        text: 'Esta acción puede tener un impacto importante. ¿Estás seguro de continuar?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sí, continuar',
        cancelButtonText: 'Cancelar',
    }).then((result) => {
        if (result.isConfirmed) {

            var formData = new FormData();

            formData.append('vista', $(event).data('vista'));
            formData.append('permiso', $(event).data('permiso'));
            formData.append('rol', $(event).data('rol'));
            formData.append('accion', $(event).data('accion'));

            if ($(event).is(':checked')) {
                formData.append('activo', true);
            } else {
                formData.append('activo', false);
            }

            $.ajax({
                url: 'ActualizarPermiso',
                async: true,
                cache: false,
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (data) {

                    if (data.error == false) {
                        Swal.fire(
                            'Asignación de Permisos',
                            'Se ha actualizado el permiso con éxito.',
                            'success'
                        )
                        datatable.ajax.reload();

                    } else {
                        Swal.fire(
                            'Asignación de Permisos',
                            'Hubo un problema al asignar el permiso, verifique su existencia.',
                            'error'
                        )
                        datatable.ajax.reload();
                    }
                },
                error: function (xhr, status, error) {
                }
            });
        }
        else {
            $(event).prop('checked', !isChecked);
        }
    });
}

function CrearEstructuraCheck(Activo, Vista, IdPermiso, idRol, Accion, Label) {
    var Cadena = "";

    Cadena += "<li><label class='form-check form-check-custom form-check-inline form-check-solid me-5 is-valid'>";
    Cadena += "<input onchange='GestionarPermisos(this)' ";
    Cadena += "data-vista='" + Vista + "' ";
    Cadena += "data-permiso='" + IdPermiso + "' ";
    Cadena += "data-rol='" + idRol + "' ";
    Cadena += "data-accion='" + Accion + "' ";
    if (Activo == true) {
        Cadena += "class='form-check-input' type='checkbox' checked>";
    } else {
        Cadena += "class='form-check-input' type='checkbox'>"
    }
    Cadena += "<span class='fw-semibold ps-2 fs-6'>"+Label+"</span>";
    Cadena += "</label></li>";

    return Cadena;
}

$("#GuardarRol").click(function () {

    const inputNombreRol = document.getElementById('create_rol');
    const rol = inputNombreRol.value;

    const regex = /^[a-zA-Z]{2,}$/;

    if (!regex.test(rol)) {
        toastr.warning("El rol debe contener al menos dos letras y no puede incluir números ni caracteres especiales.");
        return false;
    } 

    $.ajax({
        url: 'CrearNuevoRol',
        async: true,
        cache: false,
        type: 'POST',
        data: { descripcion : rol},
        success: function (data) {

            $('#nuevoRolModal').modal('toggle');
            $('#create_rol').val('');

            if (data.error == false) {
                Swal.fire(
                    'Crear Nuevo Rol',
                    'Se ha creado el rol con éxito.',
                    'success'
                )
                datatable.ajax.reload();
            } else {
                Swal.fire(
                    'Crear Nuevo Rol',
                    'Hubo un problema al crear el rol, verifique su existencia.',
                    'error'
                )
            }

        },
        error: function (xhr, status, error) {
        }
    });

    datatable.ajax.reload();
    recargar();
});

$("#create_rol").keydown(function (event) {
    if (event.keyCode === 13) { 
        event.preventDefault(); 
        $("#GuardarRol").click();
    }
});


$('#nuevoRolModal').on('hidden.bs.modal', function () {
    // Limpiar campos del modal al cerrarlo
    $('#create_rol').val('');
});