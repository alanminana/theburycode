// producto-common.js

window.cargarSubmarcas = function (marcaId, selectorSubmarca, submarcaSeleccionada = null) {
    const $submarcaSelect = $(selectorSubmarca);
    $submarcaSelect.empty().append('<option value="">-- Seleccione --</option>');

    if (!marcaId) {
        return;
    }

    $.ajax({
        url: '/Producto/GetSubmarcas',
        type: 'GET',
        data: { marcaId: marcaId },
        success: function (submarcas) {
            submarcas.forEach(function (submarca) {
                const $opt = $('<option></option>')
                    .attr('value', submarca.value)
                    .text(submarca.text);

                if (submarcaSeleccionada && submarca.value == submarcaSeleccionada) {
                    $opt.attr('selected', 'selected');
                }

                $submarcaSelect.append($opt);
            });
        },
        error: function () {
            console.error('Error al cargar submarcas para marcaId=' + marcaId);
        }
    });
};

window.calcularPrecios = function (preciosActuales = null) {
    const costo = parseFloat($('#PrecioCosto').val()) || 0;
    const margen = parseFloat($('#MargenVentaPct').val()) || 0;
    const descuento = parseFloat($('#DescuentoContadoPct').val()) || 0;
    const iva = parseFloat($('#IvaPct').val()) || 21;

    const precioLista = costo * (1 + margen / 100);
    const precioContado = precioLista * (1 - descuento / 100);
    const precioFinal = precioContado * (1 + iva / 100);

    $('#precioLista').text(precioLista.toFixed(2));
    $('#precioContado').text(precioContado.toFixed(2));
    $('#precioFinal').text(precioFinal.toFixed(2));

    if (preciosActuales) {
        let cambio = false;
        if (Math.abs(precioLista - preciosActuales.lista) > 0.01) cambio = true;
        if (Math.abs(precioContado - preciosActuales.contado) > 0.01) cambio = true;
        if (Math.abs(precioFinal - preciosActuales.final) > 0.01) cambio = true;

        if (cambio) {
            $('#preciosNuevos').show();
        } else {
            $('#preciosNuevos').hide();
        }
    } else {
        if (costo > 0) {
            $('#preciosCalculados').show();
        } else {
            $('#preciosCalculados').hide();
        }
    }
};