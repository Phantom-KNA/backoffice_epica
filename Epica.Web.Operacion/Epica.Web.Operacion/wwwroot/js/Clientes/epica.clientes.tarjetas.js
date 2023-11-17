"use strict"
toastr.options.preventDuplicates = true;

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
            processing: false,
            serverSide: true,
            filter: true,
            ordering: false,
            ajax: {
                url: siteLocation + 'Clientes/ConsultaTarjetas',
                type: 'POST',
                error: function (jqXHR, textStatus, errorThrown) {
                    $(".dataTables_processing").hide();
                    toastr.error("Se agoto el tiempo de espera para consultar estos datos. Inténtelo más tarde.");
                },
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
                { data: "proxyNumber", name: "proxyNumber", title: "Proxy" },
                { data: "tipoProducto", name: "tipoProducto", title: "Tipo Producto" }
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

        var numeroTarjeta = $('#numeroTarjeta').val();
        var proxyNumber = $('#proxyNumber').val();
        var mesVigencia = $('#mesVigencia').val();
        var yearVigencia = $('#yearVigencia').val(); 

        if (!numeroTarjeta || !proxyNumber || !mesVigencia || !yearVigencia) {
            toastr.error('Por favor, complete todos los campos obligatorios.', "").preventDuplicates;
            return; 
        }

        var primerCaracter = proxyNumber.charAt(0);
        var todosIguales = true;

        // Comprobar si todos los caracteres son iguales al primer carácter
        for (var i = 0; i < proxyNumber.length; i++) {
            if (proxyNumber.charAt(i) !== primerCaracter) {
                todosIguales = false;
                break;
            }
        }

        if (todosIguales) {
            toastr.error('Los valores ingresados, no son válidos.', "").preventDuplicates;
            return; 
        }

        var num = numeroTarjeta;
        var charCount = numeroTarjeta.length;

        if (charCount == 16) {
            var valid = isValid(num, charCount);
            if (!valid) {
                toastr.error('La tarjeta ingresada, no es válida.', "").preventDuplicates;
                return;
            }
        } else {
            toastr.error('La tarjeta ingresada, no es válida.', "").preventDuplicates;
            return;
        }

        var validaAñoActual = new Date().getFullYear();
        var validaAñoingreso = parseInt(yearVigencia)

        if (validaAñoingreso < validaAñoActual) {
            toastr.error('El año ingresado, no es válido o es menor al año en curso.', "").preventDuplicates;
            return;
        }

        toastr.info('Almacenando Tarjeta...', "").preventDuplicates;

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
                        'Se guardó la información de manera exitosa.',
                        'success'
                    )
                }
            },
            error: function (xhr, status, error) {
            }


        });   
        $("#btnCerrarCuenta").click();
    });

    function isValid(ccNum, charCount) {
        var double = true;
        var numArr = [];
        var sumTotal = 0;
        for (var i = 0; i < charCount; i++) {
            var digit = parseInt(ccNum.charAt(i));

            if (double) {
                digit = digit * 2;
                digit = toSingle(digit);
                double = false;
            } else {
                double = true;
            }
            numArr.push(digit);
        }

        for (i = 0; i < numArr.length; i++) {
            sumTotal += numArr[i];
        }
        var diff = eval(sumTotal % 10);
        return (diff == "0");
    }

    function toSingle(digit) {
        if (digit > 9) {
            var tmp = digit.toString();
            var d1 = parseInt(tmp.charAt(0));
            var d2 = parseInt(tmp.charAt(1));
            return (d1 + d2);
        } else {
            return digit;
        }
    }

    $('#kt_modal_1').on('hidden.bs.modal', function () {
        // Limpiar campos del modal al cerrarlo
        $('#numeroTarjeta').val('');
        $('#proxyNumber').val('');
        $('#mesVigencia').val('');
        $('#yearVigencia').val(''); 
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

    var ErrorControl = function () {
        dataTable.ext.errMode = 'throw';
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
        },
        ErrorControl: function () {
            ErrorControl();
        },
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

$(document).on("click", "#Redireccion", function () {
    KTApp.showPageLoading();
});

$('#kt_datatable').on('processing.dt', function (e, settings, processing) {
    if (processing) {
        KTApp.showPageLoading();
    } else {
        KTApp.hidePageLoading();
    }
})