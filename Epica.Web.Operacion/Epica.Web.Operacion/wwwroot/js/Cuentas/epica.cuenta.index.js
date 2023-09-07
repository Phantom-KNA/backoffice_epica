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
                    data: 'noCuenta', name: 'NoCuenta', title: 'Numero de Cuenta',
                    render: function (data, type, row) {
                        var partes = data.split("|"); // Separar la parte entera y decimal
                        var NumCuenta = partes[0];
                        var idCuenta = partes[1];
                        var idCliente = partes[2];
                        return "<a title='Consultar Movimientos de cuenta' href='/Clientes/Detalle/Movimientos?id=" + idCuenta + "&cliente=" + idCliente + "&noCuenta=" + NumCuenta + "'>" + NumCuenta + "</a>";
                    }
                },
                { data: 'nombrePersona', name: 'nombrePersona', title: 'Nombre persona' },
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
        const documentTitle = 'Cuentas';
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

                toastr.info('Consultando Cobranza Referenciada', "");

                $.ajax({
                    url: siteLocation + 'Cuenta/ConsultarSubCuentas',
                    async: true,
                    cache: false,
                    type: 'POST',
                    data: { 'id': TranID },
                    success: function (data) {


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

                            console.log(d);
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
                                    newTemplate.querySelectorAll('td')[4].classList.add(...borderClasses);
                                }
                                if (index === 0) { // last row
                                    let borderClasses = ['rounded-start', 'rounded-top-0'];
                                    newTemplate.querySelectorAll('td')[0].classList.add(...borderClasses);
                                    borderClasses = ['rounded-end', 'rounded-top-0'];
                                    newTemplate.querySelectorAll('td')[4].classList.add(...borderClasses);

                                    newTemplate.classList.add('border-bottom-0');
                                }
                            }

                            var tbody = document.querySelector('tbody');
                            tbody.insertBefore(newTemplate, row.nextSibling);
                        });

                        row.classList.add(...rowClasses);
                        button.classList.add('active');
                    }
                });
            }
        });
    });

}


function GestionarCuenta(AccountID, estatus) {

    Swal.fire({
        title: 'Bloqueo/Desbloqueo de Cuenta',
        text: "¿Esta seguro que desea bloquear o desbloquear esta cuenta?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Aceptar'
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: siteLocation + 'Clientes/GestionarEstatusClienteTotal',
                async: true,
                cache: false,
                type: 'POST',
                data: { id: AccountID, Estatus: estatus },
                success: function (data) {

                    datatable.ajax.reload();
                    Swal.fire(
                        'Estatus Actualizado',
                        'Se ha actualizado el estatus de la cuenta con exito.',
                        'success'
                    )
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        }
    })
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

//function populateTemplate(dataMapper, target){
//    console.log(dataMapper);
//    dataMapper.forEach((d, index) => {
//        // Clone template node
//        const newTemplate = template.cloneNode(true);

//        // Stock badges
//        const lowStock = `<div class="badge badge-light-warning">Low Stock</div>`;
//        const inStock = `<div class="badge badge-light-success">In Stock</div>`;

//        // Select data elements
//        const id = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_id"]');
//        const numcuenta = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_numero_cuenta"]');
//        const cliente = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_cliente"]');
//        const estatus = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_estatus"]');
//        const saldo = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_saldo"]');
//        const tipo = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_tipo"]');

//        // Mapeo de Datos
//        id.innerText = d.id;
//        numcuenta.innerText = d.noCuenta;
//        cliente.innerText = d.cliente;
//        estatus.innerText = d.estatus;
//        saldo.innerText = d.saldo;
//        tipo.innerText = d.tipo;

//        if (dataMapper.length === 1) {
//            let borderClasses = ['rounded', 'rounded-end-0'];
//            newTemplate.querySelectorAll('td')[0].classList.add(...borderClasses);
//            borderClasses = ['rounded', 'rounded-start-0'];
//            newTemplate.querySelectorAll('td')[4].classList.add(...borderClasses);

//            newTemplate.classList.add('border-bottom-0');
//        } else {
//            if (index === (dataMapper.length - 1)) { // first row
//                let borderClasses = ['rounded-start', 'rounded-bottom-0'];
//                newTemplate.querySelectorAll('td')[0].classList.add(...borderClasses);
//                borderClasses = ['rounded-end', 'rounded-bottom-0'];
//                newTemplate.querySelectorAll('td')[4].classList.add(...borderClasses);
//            }
//            if (index === 0) { // last row
//                let borderClasses = ['rounded-start', 'rounded-top-0'];
//                newTemplate.querySelectorAll('td')[0].classList.add(...borderClasses);
//                borderClasses = ['rounded-end', 'rounded-top-0'];
//                newTemplate.querySelectorAll('td')[4].classList.add(...borderClasses);

//                newTemplate.classList.add('border-bottom-0');
//            }
//        }

//        const tbody = document.querySelector('tbody');
//        tbody.insertBefore(newTemplate, target.nextSibling);
//    });
//}