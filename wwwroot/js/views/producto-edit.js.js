function initProductoEdit(marcaActual, submarcaActual, preciosActuales) {
    // Cargar submarcas iniciales si hay marca seleccionada
    if (marcaActual > 0) {
        cargarSubmarcas(marcaActual, submarcaActual);
    }

    // Cargar submarcas cuando se selecciona marca
    $('#MarcaId').on('change', function () {
        var marcaId = $(this).val();
        cargarSubmarcas(marcaId, null);
    });

    // Calcular precios en tiempo real
    function calcularPrecios() {
        var costo = parseFloat($('#PrecioCosto').val()) || 0;
        var margen = parseFloat($('#MargenVentaPct').val()) || 0;
        var descuento = parseFloat($('#DescuentoContadoPct').val()) || 0;
        var iva = parseFloat($('#IvaPct').val()) || 21;

        var precioLista = costo * (1 + margen / 100);
        var precioContado = precioLista * (1 - descuento / 100);
        var precioFinal = precioContado * (1 + iva / 100);

        $('#precioLista').text(precioLista.toFixed(2));
        $('#precioContado').text(precioContado.toFixed(2));
        $('#precioFinal').text(precioFinal.toFixed(2));

        // Comparar con precios actuales
        var cambio = false;
        if (Math.abs(precioLista - preciosActuales.lista) > 0.01) cambio = true;
        if (Math.abs(precioContado - preciosActuales.contado) > 0.01) cambio = true;
        if (Math.abs(precioFinal - preciosActuales.final) > 0.01) cambio = true;

        if (cambio) {
            $('#preciosNuevos').show();
        } else {
            $('#preciosNuevos').hide();
        }
    }

    $('#PrecioCosto, #MargenVentaPct, #DescuentoContadoPct, #IvaPct').on('input', calcularPrecios);
}
