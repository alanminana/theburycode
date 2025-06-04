function initProductoCreate() {
    $('#MarcaId').on('change', function () {
        var marcaId = $(this).val();
        cargarSubmarcas(marcaId, '#SubmarcaId');
    });

    $('#PrecioCosto, #MargenVentaPct, #DescuentoContadoPct, #IvaPct').on('input', function () {
        calcularPrecios();
    });
}