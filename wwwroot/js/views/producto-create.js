function initProductoCreate(marcaId) {
    // Cargar submarcas cuando se selecciona marca
    $('#MarcaId').on('change', function () {
        var marcaId = $(this).val();
        var submarcaSelect = $('#SubmarcaId');

        submarcaSelect.empty();
        submarcaSelect.append('<option value="">-- Seleccione --</option>');

        if (marcaId) {
            $.ajax({
                url: '/Producto/GetSubmarcas',
                type: 'GET',
                data: { marcaId: marcaId },
                success: function (submarcas) {
                    $.each(submarcas, function (index, submarca) {
                        submarcaSelect.append($('<option></option>')
                            .attr('value', submarca.value)
                            .text(submarca.text));
                    });
                }
            });
        }
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

        if (costo > 0) {
            $('#preciosCalculados').show();
        }
    }

    $('#PrecioCosto, #MargenVentaPct, #DescuentoContadoPct, #IvaPct').on('input', calcularPrecios);
}
