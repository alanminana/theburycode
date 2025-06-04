function initProductoEdit(marcaActual, submarcaActual, preciosActuales) {
    if (marcaActual > 0) {
        cargarSubmarcas(marcaActual, '#SubmarcaId', submarcaActual);
    }

    $('#MarcaId').on('change', function () {
        var marcaId = $(this).val();
        cargarSubmarcas(marcaId, '#SubmarcaId');
    });

    $('#PrecioCosto, #MargenVentaPct, #DescuentoContadoPct, #IvaPct').on('input', function () {
        calcularPrecios(preciosActuales);
    });
}