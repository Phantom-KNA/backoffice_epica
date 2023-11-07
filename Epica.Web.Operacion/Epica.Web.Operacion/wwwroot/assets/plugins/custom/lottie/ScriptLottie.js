KTApp.showPageLoading();
animation = bodymovin.loadAnimation({
    container: document.getElementById('layout'),
    renderer: 'svg',
    loop: true,
    autoplay: true,
    path: '/assets/plugins/custom/lottie/loading.json',

});

//layout.addEventListener('complete', function () {
//    animation.play();
//});


$(document).ready(function () {
    var tablasCargadas = 0;
    var tablasTotales = 0;

    initializeDataTables();

    function initializeDataTables() {
        $('.dataTable:not(.excludeDatatable)').each(function () {
            tablasTotales++;
            var tabla = $(this).DataTable();

            tabla.on('preXhr.dt', function () {
                KTApp.showPageLoading();
            });

            tabla.on('draw.dt', function (e, settings, processing) {
                tablasCargadas++;
                if (tablasCargadas >= tablasTotales) {
                    KTApp.hidePageLoading();
                }
            });

        });

        if (tablasTotales === 0) {
            KTApp.hidePageLoading();
        }
    }
});
