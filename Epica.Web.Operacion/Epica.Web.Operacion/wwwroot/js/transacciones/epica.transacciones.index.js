"use strict";

//Agregar la tabla de transacciones
var KTDatatableTransacciones = (function () {
    let datatable_myAccounts;
    var table;

    var init = function () {
        datatable_myAccounts = $("#kt_datatable_transacciones").DataTable({
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
            ordering: true,
            filter: true,
            // Configuración de la fuente de datos AJAX
            ajax: {
                url: "Transacciones/Transacciones",
                type: "GET",
                dataSrc: "", // La respuesta del controlador debe ser un array de movimientos
            },
            // Configuración de las columnas
            columns: [
                { data: "claveRastreo", name: "ClaveRastreo", title: "CLAVE DE RASTREO" },
                { data: "idInterno", name: "IdInterno", title: "ID INTERNO" },
                { data: "nombreCuenta", name: "NombreCuenta", title: "NOMBRE DE LA CUENTA" },
                { data: "institucion", name: "Institucion", title: "INSTITUCION" },
                {
                    data: "monto",
                    name: "Monto",
                    title: "MONTO",
                    render: function (data, type, row) {
                        var color = 'black';//Color por defecto
                        if (row.monto > 0) {
                            color = 'text-success';//Color verde si es mayor a 0 
                        } else if (row.monto < 0) {
                            color = 'text-danger';//Color rojo si es menor a 0
                        }
                        return '<pan class ="' + color + '">' + "$" + data + '<s/span>';
                    }
                },
                {
                    data: "estatus",
                    name: "Estatus",
                    title: "ESTATUS",
                    render: function (data, type, row) {
                        var color = 'badge badge-light-danger';//Color por defecto
                        if (row.estatus == "Liquidado") {
                            color = 'badge badge-light-primary';//Color verde si es mayor a 0 
                        } else if (row.estatus == "En Tránsito") {
                            color = 'badge badge-light-warning';//Color rojo si es menor a 0
                        }
                        return '<span class ="' + color + '">' + data + '</span>';
                    }
                },
                { data: "concepto", name: "Concepto", title: "CONCEPTO" },
                {
                    data: "medioPago",
                    name: "MedioPago",
                    title: "MEDIO DE PAGO",
                    render: function (data, type, row) {
                        var medio = 'text';
                        if (row.medioPago == 0) {
                            medio = 'Tarjeta de Credito';
                        } else if (row.medioPago == 1) {
                            medio = 'Tarjeta de Debito';
                        } else if (row.medioPago == 2) {
                            medio = 'Transferencia';
                        } else {
                            medio = 'Efectivo';
                        }
                        return '<span> ' + medio + '</span>';
                    }
                },    
                {
                    data: "tipo",
                    name: "Tipo",
                    title: "TIPO",
                    render: function (data, type, row) {
                        var tipo = 'text';
                        if (row.tipo == 0) {
                            tipo = 'Ingreso';
                        } else {
                            tipo = 'Egreso';
                        }
                        return '<span> ' + tipo + '</span>';
                    }
                },
                {
                    data: "fecha",
                    name: "Fecha",
                    title: "FECHA",
                    render: function (data) {
                        var formatoFecha = moment(data).format("DD/MM/YYYY");
                        return formatoFecha;
                    }
                },
                {
                    title: "ACCIONES",
                    orderable: false,
                    data: null,
                    defaultContent: "",
                    render: function (data, type, row) {
                        // Renderización del contenido HTML de la columna de acciones
                        if (type === "display") {
                            var acciones = '<div class="dropdown dropdown-inline"> <button type="button" class="btn btn-sm btn-icon btn-light btn-active-light-primary toggle h-25px w-25px" data-kt-docs-datatable-subtable="expand_row"> <i class="ki-duotone ki-plus fs-3 m-0 toggle-off"></i><i class="ki-duotone ki-minus fs-3 m-0 toggle-on"></i> </button> <a href="javascript:;" class="btn btn-sm btn-icon btn-light btn-active-light-primary me-2" data-bs-toggle="dropdown"> <i class="bi bi-three-dots"></i> </a> <ul class="dropdown-menu menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-semibold fs-7 w-150px py-4"> <li class="menu-item px-3"> <a data-id="" asp-controller="Cuenta" asp-action="ConsultarTransacciones" asp-route-id="@Model.Id" class="btnEditar menu-link px-3"> <i class="bi bi-search"></i>&nbsp; Comprobante </a> </li> <li class="menu-item px-3"> <a data-id="@Model.Id" asp-controller="Cuenta" asp-action="ConsultarTransacciones" asp-route-id="@Model.Id" class="btnEditar menu-link px-3" id="btnOpenModal"> <i class="bi bi-search"></i>&nbsp; Detalles </a> </li> </ul> </div > ';
                        return acciones;
                        }
                    },
                },
            ],
        });
    };

    var detalle = function () {
        $("#btnOpenModal").click(function () {
            $.ajax({
                url: "url_del_servidor",
                type: "GET",
                success: function (response) {
                    // Mostrar el modal después de que se complete la solicitud AJAX
                    $("#myModal").modal("show");
                },
                error: function (xhr, status, error) {
                    console.log("Error en la solicitud AJAX: " + error);
                }
            });
        });
    };


    // Función de busqueda 
    var handleSearchDatatable = function () {
        const filterSearch = document.querySelector('[data-kt-myaacounts-table-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable_myAccounts.search(e.target.value).draw();
        });
    };
    // Función para descarga del archivo
    //var Download = function (idTransaccion) {
    //    var url = "Home/Archivo?idTransaccion=" + idTransaccion;
    //    window.location.href = url;
    //};

    //document.getElementById('filterForm').addEventListener('submit', function (event) {
    //    event.preventDefault(); // Evita que el formulario se envíe normalmente

    //    var form = event.target;
    //    var formData = new FormData(form);

    //    fetch('/url-del-endpoint', {
    //        method: 'POST', // Utiliza el método POST para enviar los datos del formulario
    //        body: formData
    //    })
    //        .then(response => response.json())
    //        .then(data => {
    //            // Manipula los datos recibidos y actualiza la interfaz de usuario
    //            console.log(data);

    //            // Ejemplo: Actualiza el contenido en el contenedor de resultados
    //            var resultContainer = document.getElementById('resultContainer');
    //            resultContainer.innerHTML = '';

    //            data.forEach(item => {
    //                var itemElement = document.createElement('div');
    //                itemElement.textContent = item.name;
    //                resultContainer.appendChild(itemElement);
    //            });
    //        })
    //        .catch(error => {
    //            // Maneja los errores de la petición aquí
    //            console.error(error);
    //        });
    //});

    // Función de recarga
    var reload = function () {
        datatable_myAccounts.ajax.reload();
    };

    return {
        init: function () {
            table = document.querySelector("#kt_datatable_transacciones");

            //$(document.body).on('click', '#download-btn', function () {
            //    var idTransaccion = $(this).data('idtransaccion');
            //    Download(idTransaccion);
            //});

            if (!table) {
                return;
            }
            init();
            handleSearchDatatable();
            detalle();
        },
        reload: function () {
            reload();
        },
    };
})();
$(document).ready(function () {
    KTDatatableTransacciones.init();
    var model = Html.Raw(ViewBag.AccountID);
    console.log(model);
});

