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
                { data: 'idCuenta', name: 'idCuenta', title: 'ID' },
                { data: 'noCuenta', name: 'NoCuenta', title: 'Numero de Cuenta' },
                { data: 'nombrePersona', name: 'nombrePersona', title: 'Cliente' },
                {
                    data: 'estatus', name: 'Estatus', title: 'Estatus',
                    render: function (data, type, row) {
                        return data == 1 ?
                            "<span class='badge badge-light-success' >Activo</span>" : "<span class='badge badge-light-danger' >Desactivado</span>";
                    }
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
                        columns: [0, 1, 2, 3, 4, 5]
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
        const filterSearch = document.querySelector('[data-kt-customer-table-filter="search"]');
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

function pruebas(id) {

    var dataInfo = []; 
    dataInfo.length = 0;

    $.ajax({
        url: siteLocation + 'Cuenta/ConsultarSubCuentas',
        async: true,
        cache: false,
        contentType: false,
        processData: false,
        type: 'POST',
        success: function (data) {
            
            var buttonsSubTable = document.querySelectorAll('[data-kt-docs-datatable-subtable="expand_row"]');
            buttonsSubTable.forEach((button, index) => {
                button.addEventListener('click', e => {
                    //e.stopImmediatePropagation();
                    //e.preventDefault();
                    var row = button.closest('tr');
                    var rowClasses = ['isOpen', 'border-bottom-0'];

                    if (row.classList.contains('isOpen')) {
                        while (row.nextSibling && row.nextSibling.getAttribute('data-kt-docs-datatable-subtable') === 'subtable_template') {
                            row.nextSibling.parentNode.removeChild(row.nextSibling);
                        }
                        row.classList.remove(...rowClasses);
                        button.classList.remove('active');
                    } else {

                        data.forEach((d, index) => {
                            // Clone template node
                            var newTemplate = template.cloneNode(true);

                            // Select data elements
                            var id = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_id"]');
                            var numcuenta = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_numero_cuenta"]');
                            var cliente = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_cliente"]');
                            var estatus = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_estatus"]');
                            var saldo = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_saldo"]');
                            var tipo = newTemplate.querySelector('[data-kt-docs-datatable-subtable="template_tipo"]');

                            // Mapeo de Datos
                            id.innerText = d.idCuenta;
                            numcuenta.innerText = d.noCuenta;
                            cliente.innerText = d.nombrePersona;
                            estatus.innerHTML = d.estatus == 1 ? "<span class='badge badge-light-success'>Activo</span>" : "<span class='badge badge-light-danger'>Desactivado</span>";
                            saldo.innerText = accounting.formatMoney(d.saldo);
                            tipo.innerText = d.tipoPersona;

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
            });
        }
    });

}


function GestionarCuenta(AccountID, estatus) {
    $.ajax({
        url: siteLocation + 'Cuenta/GestionarEstadoCuentas',
        async: true,
        cache: false,
        type: 'POST',
        data: { id: AccountID, Estatus: estatus },
        success: function (data) {

            datatable.ajax.reload();
            alert("Se ha actualizado la cuenta con exito.");
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}


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