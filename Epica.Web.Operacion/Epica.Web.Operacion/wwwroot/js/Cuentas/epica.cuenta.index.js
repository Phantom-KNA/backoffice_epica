toastr.options.preventDuplicates = true;
var table;
var datatable;
var filterAccount;



var KTDatatableRemoteAjax = function () {
    var table;
    $(".filtro-control").val('');
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
                url: siteLocation + 'Cuenta/Consulta',
                type: 'POST',
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
/*                { data: 'idCuenta', name: 'idCuenta', title: 'ID' },*/
                {
                    data: 'clabe', name: 'Clabe', title: 'Cuenta Clabe'
                },
                {
                    data: 'noCuenta', name: 'NoCuenta', title: 'Número de Cuenta',
                    render: function (data, type, row) {
                        var partes = data.split("|"); // Separar la parte entera y decimal
                        var NumCuenta = partes[0];
                        var idCuenta = partes[1];
                        var idCliente = partes[2];
                        return "<a title='Consultar Movimientos de cuenta' href='/Clientes/Detalle/Movimientos?id=" + idCuenta + "&cliente=" + idCliente + "&noCuenta=" + NumCuenta + "'>" + NumCuenta + "</a>";
                    }
                },
                {
                    data: 'nombrePersona', name: 'nombrePersona', title: 'Nombre Persona'
                },
                {
                    data: 'saldo', name: 'Saldo', title: 'Saldo',
                    render: function (data, type, row) {
                        return accounting.formatMoney(data);
                    }
                },
                {
                    data: 'tipoPersona', name: 'Tipo', title: 'Tipo Persona'
                },
                {
                    data: 'estatus', name: 'Estatus', title: 'Estatus Cuenta',
                    render: function (data, type, row) {
                        return data == 1 ?
                            "<span class='badge badge-light-danger' >Desactivado</span>" : "<span class='badge badge-light-success' >Activo</span>";
                    }
                },
                {
                    data: 'bloqueoSPEIOut', name: 'BloqueoSpeiOut', title: 'Estatus Spei Out',
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
                            htmlString = '<div class="custom-cell" >' + htmlString + '</div>';
                            return htmlString;
                        }
                    },
                }
            ],
        });
        $('thead tr').addClass('text-start text-gray-400 fw-bold fs-7 text-uppercase gs-0');

    };

    var exportButtons = () => {
        const documentTitle = 'Cuentas';
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6]
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

    $(".btn-filtrar").click(function () {
        recargar();
    })

    $(".btn_limpiar_filtros").click(function () {
        $(".filtro-control").val('');
        $(".filtro-control-select").val(null).trigger('change');;
        recargar();
    });

    // Template de subtabla
    const subtable = document.querySelector('[data-kt-docs-datatable-subtable="subtable_template"]');
    template = subtable.cloneNode(true);
    template.classList.remove('d-none');

    // Remove subtable template
    subtable.parentNode.removeChild(subtable);


    // Reset subtable
    const resetSubtable = () => {
        const subtables = document.querySelectorAll('[data-kt-docs-datatable-subtable="subtable_template"]');
        subtables.forEach(st => {
            st.parentNode.removeChild(st);
        });

        const rows = table.querySelectorAll('tbody tr');
        rows.forEach(r => {
            r.classList.remove('isOpen');
            if (r.querySelector('[data-kt-docs-datatable-subtable="expand_row"]')) {
                r.querySelector('[data-kt-docs-datatable-subtable="expand_row"]').classList.remove('active');
            }
        });
    }

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
            resetSubtable();
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

function pruebas(idAccount) {
    var dataInfo = []; 
    dataInfo.length = 0;
    TranID = idAccount;
    var buttonsSubTable = document.querySelectorAll('[data-kt-docs-datatable-subtable="expand_row"]');
    buttonsSubTable.forEach((button, index) => {
        button.addEventListener('click', e => {
            e.stopImmediatePropagation();
            e.preventDefault();

            var row = button.closest('tr');
            var rowClasses = ['isOpen', 'border-bottom-0'];

            if (row.classList.contains('isOpen')) {
                while (row.nextSibling && row.nextSibling.getAttribute('data-kt-docs-datatable-subtable') === 'subtable_template') {
                    row.nextSibling.parentNode.removeChild(row.nextSibling);
                }
                row.classList.remove(...rowClasses);
                button.classList.remove('active');
            } else {

                toastr.info('Consultando Cobranza Referenciada', "").preventDuplicates;

                $.ajax({
                    url: siteLocation + 'Cuenta/ConsultarSubCuentas',
                    async: true,
                    cache: false,
                    type: 'POST',
                    data: { 'id': TranID },
                    success: function (data) {

                        if (data.length != 0) {
                            data.forEach((d, index) => {
                                // Clone template node
                                var newTemplate = template.cloneNode(true);

                                // Select data elements
                                var numcuenta = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_numero_cuenta"]');
                                var cliente = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_cliente"]');
                                var estatus = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_estatus"]');
                                var mediopago = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_medio_pago"]');
                                var noreferencia = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_numero_referencia"]');
                                var fechadato = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_fecha"]');
                                var numcuentaclabe = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_cuenta_clabe"]');
                                var alias = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_alias"]');
                                var numerotatarjeta = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_numero_tarjeta"]');

                                // Mapeo de Datos
                                numcuenta.innerText = d.noCuentaPadre;

                                if (d.nombre == "") {
                                    cliente.innerText = "N/A";
                                } else {
                                    cliente.innerText = d.nombre;
                                }


                                if (d.estatus == null) {
                                    estatus.innerHTML = "<br> <span class='badge badge-light-warning'>En Proceso</span>"
                                } else if (d.estatus == 0) {
                                    estatus.innerHTML = "<br> <span class='badge badge-light-success'>Liquidado</span>"
                                } else if (d.estatus == 1) {
                                    estatus.innerHTML = "<br> <span class='badge badge-light-danger'>Error</span>"
                                } else {
                                    estatus.innerHTML = "<br> <span class='badge badge-light-danger'>Error</span>"
                                }
                                //estatus.innerHTML = d.estatus == 1 ? "<span class='badge badge-light-success'>Activo</span>" : "<span class='badge badge-light-danger'>Desactivado</span>";
                                mediopago.innerText = d.descripcionPago;

                                if (d.numeroReferencia == null) {
                                    noreferencia.innerText = "N/A";
                                } else {
                                    noreferencia.innerText = d.numeroReferencia;
                                }

                                fechadato.innerText = d.fechaAlta;

                                numcuentaclabe.innerText = d.cuentaClabe;
                                alias.innerText = d.alias;
                                numerotatarjeta.innerText = d.noTarjeta;

                                if (data.length === 1) {
                                    let borderClasses = ['rounded', 'rounded-end-0'];
                                    newTemplate.querySelectorAll('td')[0].classList.add(...borderClasses);
                                    borderClasses = ['rounded', 'rounded-start-0'];
                                    newTemplate.querySelectorAll('td')[4].classList.add(...borderClasses);

                                    newTemplate.classList.add('border-bottom-0');
                                } else {
                                    if (index === (data.length - 1)) { // first row
                                        let borderClasses = ['rounded-start', 'rounded-bottom-0'];
                                        newTemplate.querySelectorAll('td')[0].classList.add(...borderClasses);
                                        borderClasses = ['rounded-end', 'rounded-bottom-0'];
                                        newTemplate.querySelectorAll('td')[7].classList.add(...borderClasses);
                                    }
                                    if (index === 0) { // last row
                                        let borderClasses = ['rounded-start', 'rounded-top-0'];
                                        newTemplate.querySelectorAll('td')[0].classList.add(...borderClasses);
                                        borderClasses = ['rounded-end', 'rounded-top-0'];
                                        newTemplate.querySelectorAll('td')[7].classList.add(...borderClasses);
                                        
                                        newTemplate.classList.add('border-bottom-0');

                                    }
                                }

                                var tbody = document.querySelector('tbody');
                                tbody.insertBefore(newTemplate, row.nextSibling);
                            });

                            row.classList.add(...rowClasses);
                            button.classList.add('active');

                            if (document.documentElement.getAttribute("data-theme") == "light") {
                                $(".subtables").css("background-color", "aliceblue");
                            } else {
                                $(".subtables").css("background-color", "#848751");
                            }

                        } else {
                            toastr.warning('No se encontró cobranza referenciada para esta cuenta.', "").preventDuplicates;
                        }

                    }
                });
            }
        });
    });

}




function GestionarCuenta(AccountID, estatus) {
    var token = "";
    var CS = "";
    Swal.fire({
        title: '<span class="swal-title-custom"><b>Valida tu identidad</b></span>',
        text: 'Por favor, ingrese su token y código de seguridad:',
        html:
            '<label for="swal-input1" class="swal-label"><b>Token:</b></label>' +
            '<input id="swal-input1" class="swal2-input" style="font-size:14px;width:250px"  placeholder="Token" oninput="validateInput(this)" maxlength="6">' +
            '<div style="margin-top: 20px;"></div>' +
            '<label for="swal-input2" class="swal-label"><b>Código de Seguridad:</b></label>' +
            '<input id="swal-input2" class="swal2-input" style="font-size:14px;width:250px" type="password" placeholder="Código de Seguridad" oninput="validateInput(this)" maxlength="6">',
        showCancelButton: true,
        confirmButtonColor: '#0493a8',
        confirmButtonText: 'Aceptar',
        cancelButtonText: 'Cancelar',
        preConfirm: () => {
            const swalInput1 = Swal.getPopup().querySelector('#swal-input1').value;
            token = Swal.getPopup().querySelector('#swal-input1').value;
            const swalInput2 = Swal.getPopup().querySelector('#swal-input2').value;
            CS = Swal.getPopup().querySelector('#swal-input2').value;
            return [swalInput1, swalInput2];
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const [tokenInput, codigoInput] = result.value;
            if (isValidInput(tokenInput) && isValidInput(codigoInput)) {

            // Realiza la validación del token y código de seguridad
            $.ajax({
                url: '/Autenticacion/ValidarTokenYCodigo',
                async: true,
                cache: false,
                type: 'POST',
                data: { token: tokenInput, codigo: codigoInput },
                success: function (validationResult) {
                    if (validationResult.mensaje === true) {
    Swal.fire({
        title: (estatus.toLowerCase() === 'true' ? 'Desbloqueo de cuenta' : 'Bloqueo de cuenta'),
        text: (estatus.toLowerCase() === 'true' ? "¿Estás seguro que deseas desbloquear esta cuenta?" : "¿Estás seguro que deseas bloquear esta cuenta?"),
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: 'Cuenta/GestionarEstatusCuentas',
                async: true,
                cache: false,
                type: 'POST',
                data: { id: AccountID, Estatus: estatus, token: token, cs: CS },
                success: function (data) {

                    if (data.error == true) {
                        Swal.fire(
                            'Estatus Actualizado',
                            'Hubo un problema al actualizar el estatus de la cuenta, verifique su existencia o Inténtalo más tarde.',
                            'error'
                        )
                    } else {
                        datatable.ajax.reload();
                        Swal.fire(
                            'Estatus Actualizado',
                            'Se ha actualizado el estatus de la cuenta con éxito.',
                            'success'
                        )
                    }
                },
                error: function (xhr, status, error) {
                }
            });
        }
    });
                    } else {
                        Swal.fire(
                            'Error',
                            'Token o código de seguridad incorrectos. Inténtalo de nuevo.',
                            'error'
                        );
                    }
                },
                error: function () {
                }
            });
            } else {
                Swal.fire(
                    'Error',
                    'Los campos Token y Código de Seguridad deben contener 6 números.',
                    'error'
                );
            }
        }
    });
}

function validateInput(inputElement) {
    inputElement.value = inputElement.value.replace(/[^0-9]/g, '');
}

function isValidInput(input) {
    return input.length === 6 && !isNaN(input);
}

function GestionarCuentaSpeiOut(AccountID, estatus) {
    var token = "";
    var CS = "";
    Swal.fire({
        title: '<span class="swal-title-custom"><b>Valida tu identidad</b></span>',
        text: 'Por favor, ingrese su token y código de seguridad:',
        html:
            '<label for="swal-input1" class="swal-label"><b>Token:</b></label>' +
            '<input id="swal-input1" class="swal2-input" style="font-size:14px;width:250px"  placeholder="Token" oninput="validateInput(this)" maxlength="6">' +
            '<div style="margin-top: 20px;"></div>' +
            '<label for="swal-input2" class="swal-label"><b>Código de Seguridad:</b></label>' +
            '<input id="swal-input2" class="swal2-input" style="font-size:14px;width:250px" type="password" placeholder="Código de Seguridad" oninput="validateInput(this)" maxlength="6">',
        showCancelButton: true,
        confirmButtonColor: '#0493a8',
        confirmButtonText: 'Aceptar',
        cancelButtonText: 'Cancelar',
        preConfirm: () => {
            const swalInput1 = Swal.getPopup().querySelector('#swal-input1').value;
            token = Swal.getPopup().querySelector('#swal-input1').value;
            const swalInput2 = Swal.getPopup().querySelector('#swal-input2').value;
            CS = Swal.getPopup().querySelector('#swal-input2').value;
            return [swalInput1, swalInput2];
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const [tokenInput, codigoInput] = result.value;
            if (isValidInput(tokenInput) && isValidInput(codigoInput)) {

            // Realiza la validación del token y código de seguridad
            $.ajax({
                url: '/Autenticacion/ValidarTokenYCodigo',
                async: true,
                cache: false,
                type: 'POST',
                data: { token: tokenInput, codigo: codigoInput },
                success: function (validationResult) {
                    if (validationResult.mensaje === true) {
                        Swal.fire({
                            title: (estatus.toLowerCase() === 'true' ? 'Desbloqueo de Spei Out' : 'Bloqueo de Spei Out'),
                            text: (estatus.toLowerCase() === 'true' ? "¿Estás seguro que deseas desbloquear el Spei Out a esta cuenta?" : "¿Estás seguro que deseas bloquear el Spei Out a esta cuenta?"),
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonColor: '#3085d6',
                            cancelButtonColor: '#d33',
                            confirmButtonText: 'Aceptar'
                        }).then((result) => {
                            if (result.isConfirmed) {

                                $.ajax({
                                    url: 'Cuenta/GestionarEstatusSpeiOutCuentas',
                                    async: true,
                                    cache: false,
                                    type: 'POST',
                                    data: { id: AccountID, Estatus: estatus, token: token, cs: CS },
                                    success: function (data) {

                                        if (data.error == true) {
                                            Swal.fire(
                                                'Estatus Actualizado',
                                                'Hubo un problema al actualizar el estatus de la cuenta, verifique su existencia o Inténtalo más tarde.',
                                                'error'
                                            )
                                        } else {
                                            datatable.ajax.reload();
                                            Swal.fire(
                                                'Estatus Actualizado',
                                                'Se ha actualizado el estatus de la cuenta con éxito.',
                                                'success'
                                            )
                                        }
                                    },
                                    error: function (xhr, status, error) {
                                    }
                                });
                            }
                        });
                    } else {
                        Swal.fire(
                            'Error',
                            'Token o código de seguridad incorrectos. Inténtalo de nuevo.',
                            'error'
                        );
                    }
                },
                error: function () {
                }
            });
            } else {
                Swal.fire(
                    'Error',
                    'Los campos Token y Código de Seguridad deben contener 6 números.',
                    'error'
                );
            }
        }
    });
}

$(document).on('click', '.btnDetalle', function (e) {
    var id = $(this).data('id');
    ModalDetalle.init(id);
});

var ModalDetalle = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "Cuenta/DetalleCuenta",
            data: { 'numCuenta': id },
            success: function (result) {
                if (result.error) {
                    $(window).scrollTop(0);
                    $("#DivSuccessMessage").hide();
                    $("#DivErrorMessage").show();
                    setTimeout(function () { $("#DivErrorMessage").hide() }, 3000);
                    $("#ErrorMessage").text(result.errorDescription);
                } else {
                    $('#modal_detalle #modalLabelTitle').html('Detalle de Cuenta');
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

$('#kt_datatable').on('show.bs.dropdown', function () {
    $('#kt_datatable').css("overflow", "inherit");
});

$('#kt_datatable').on('hide.bs.dropdown', function () {
    $('#kt_datatable').css("overflow", "auto");
})

//function SoloNumeros(e) {
//    var charCode = (e.which) ? e.which : event.keyCode;
//    if (String.fromCharCode(charCode).match(/[^0-9]/g))
//        return false;
//}
//function SoloLetras(e) {
//    var keyCode = e.keyCode || e.which;
//    var regex = /^[A-Za-z]+$/;

//    //Validate TextBox value against the Regex.
//    var isValid = regex.test(String.fromCharCode(keyCode));
//    if (!isValid) {
//        return false;
//    }

//    return isValid;

//}

function atributoThemeCambiado(mutationsList) {
    //alert("cambio de tema");
    //for (let mutation of mutationsList) {
    //    if (mutation.type === 'attributes' && mutation.attributeName === 'data-theme') {
    //        const nuevoValor = $(mutation.target).attr('data-theme');

    //        // Realizar acciones específicas cuando el atributo cambie a "light" o "dark"
    //        if (nuevoValor === 'light') {
    //            console.log('El tema cambió a light');
    //            // Realiza acciones para el tema light
    //        } else if (nuevoValor === 'dark') {
    //            console.log('El tema cambió a dark');
    //            // Realiza acciones para el tema dark
    //        }
    //    }
    //}
}

//$('#filtro_cuenta_clabe').keypress(function (e) {


//});  