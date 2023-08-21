var datatable;

var KTDatatableRemoteAjax = function () {
    var initDatatable = function () {
        datatable = $('#kt_datatable_users').DataTable({
            "order": [],
            pageLength: 15,
            language: {},
            responsive: true,
            pagingType: 'simple_numbers',
            searching: true,
            lengthMenu: [5, 10, 15, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            ajax: {
                url: '/Usuarios/ObtenerUsuariosConPermisos', // Asegúrate de que esta ruta sea correcta
                type: 'POST',
                data: function (d) {
                    // Puedes agregar parámetros adicionales si es necesario
                }
            },
            columns: [
                { data: 'idUsuario', name: 'IdUsuario', title: 'ID de Usuario' },
                { data: 'usuario', name: 'Usuario', title: 'Usuario' },
                { data: 'email', name: 'Email', title: 'Correo Electrónico' },
                { data: 'rol', name: 'Rol', title: 'Rol' },
                {
                    data: 'accionesPorModulo',
                    name: 'accionesPorModulo',
                    title: 'Acciones por Módulo',
                    render: function (data) {
                        // Aquí convertimos el objeto accionesPorModulo en una cadena
                        var acciones = data.map(function (accion) {
                            return accion.modulo + ': ' + accion.acciones.join(', ');
                        }).join('<br>');

                        return acciones;
                    }
                },
                { data: 'IsAuthenticated', name: 'IsAuthenticated', title: 'Autenticado' }
            ],
        });
    };

    return {
        init: function () {
            initDatatable();
        },
    };
}();

$(document).ready(function () {
    KTDatatableRemoteAjax.init();
});