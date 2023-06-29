"use strict";

//Agregar la tabla de transacciones
var KTDatatableTransacciones = (function () {
    let datatable_myAccounts;
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
            ordering: true,
            // Configuración de la fuente de datos AJAX
            ajax: {
                url: "Home/Transacciones",
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
                            color = 'green';//Color verde si es mayor a 0 
                        } else if (row.monto < 0) {
                            color = 'red';//Color rojo si es menor a 0
                        }
                        return '<pan style ="color: ' + color + '">' + "$" + data + '<s/span>';
                    }
                },
                { data: "estatus", name: "Estatus", title: "ESTATUS" },
                { data: "concepto", name: "Concepto", title: "CONCEPTO" },
                { data: "medioPago", name: "MedioPago", title: "MEDIO DE PAGO" },    
                { data: "tipo", name: "Tipo", title: "TIPO" },
                { data: "fecha", name: "Fecha", title: "FECHA" },
                {
                    title: "ACCIONES",
                    orderable: false,
                    data: null,
                    defaultContent: "",
                    render: function (data, type) {
                        // Renderización del contenido HTML de la columna de acciones
                        if (type === "display") {
                            var button = ' <button data-idtransaccion="' + data.idTransaccion + '" id="download-btn" type="button" class="btn btn-sm btn-light-primary me-3" data-kt-menu-placement="bottom-end" data-kt-menu-trigger="click"> <span> <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><rect opacity="0.3" width="12" height="2" rx="1" transform="matrix(0 -1 -1 0 12.75 19.75)" fill="currentColor" /><path d="M12.0573 17.8813L13.5203 16.1256C13.9121 15.6554 14.6232 15.6232 15.056 16.056C15.4457 16.4457 15.4641 17.0716 15.0979 17.4836L12.4974 20.4092C12.0996 20.8567 11.4004 20.8567 11.0026 20.4092L8.40206 17.4836C8.0359 17.0716 8.0543 16.4457 8.44401 16.056C8.87683 15.6232 9.58785 15.6554 9.9797 16.1256L11.4427 17.8813C11.6026 18.0732 11.8974 18.0732 12.0573 17.8813Z" fill="currentColor" /><path opacity="0.3" d="M18.75 15.75H17.75C17.1977 15.75 16.75 15.3023 16.75 14.75C16.75 14.1977 17.1977 13.75 17.75 13.75C18.3023 13.75 18.75 13.3023 18.75 12.75V5.75C18.75 5.19771 18.3023 4.75 17.75 4.75L5.75 4.75C5.19772 4.75 4.75 5.19771 4.75 5.75V12.75C4.75 13.3023 5.19771 13.75 5.75 13.75C6.30229 13.75 6.75 14.1977 6.75 14.75C6.75 15.3023 6.30229 15.75 5.75 15.75H4.75C3.64543 15.75 2.75 14.8546 2.75 13.75V4.75C2.75 3.64543 3.64543 2.75 4.75 2.75L18.75 2.75C19.8546 2.75 20.75 3.64543 20.75 4.75V13.75C20.75 14.8546 19.8546 15.75 18.75 15.75Z" fill="currentColor" /></svg>Descargar</span ></button > ';
                            return button;
                        }
                        return "";
                    },
                },
            ],
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

        },
        reload: function () {
            reload();
        },
    };
})();
$(document).ready(function () {
    KTDatatableTransacciones.init();
});

