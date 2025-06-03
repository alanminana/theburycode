// producto-common.js

/**
 * Función global para cargar las submarcas de una marca dada.
 * @param {number|string} marcaId                 - El ID de la marca seleccionada.
 * @param {string} selectorSubmarca               - Selector jQuery del <select> de submarcas (ej: "#SubmarcaId").
 * @param {number|string|null} submarcaSeleccionada - (Opcional) ID de la submarca que queremos pre-seleccionar.
 */
window.cargarSubmarcas = function (marcaId, selectorSubmarca, submarcaSeleccionada = null) {
    const $submarcaSelect = $(selectorSubmarca);
    // Limpio el <select> y agrego opción por defecto
    $submarcaSelect.empty().append('<option value="">-- Seleccione --</option>');

    if (!marcaId) {
        // Si no hay marcaId (vacío o null), no hago nada más.
        return;
    }

    // Petición AJAX para obtener la lista de submarcas
    $.ajax({
        url: '/Producto/GetSubmarcas',
        type: 'GET',
        data: { marcaId: marcaId },
        success: function (submarcas) {
            // submarcas es un array de objetos { value: ..., text: ... }
            submarcas.forEach(function (submarca) {
                const $opt = $('<option></option>')
                    .attr('value', submarca.value)
                    .text(submarca.text);

                // Si nos pasaron submarcaSeleccionada y coincide, lo marcamos
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


/**
 * Función global para calcular precios en tiempo real.
 * Si recibe un objeto `preciosActuales`, entra en "modo edición" y compara:
 *   preciosActuales = { lista: number, contado: number, final: number }
 * Si no recibe `preciosActuales`, asume "modo creación" y solo muestra el cuadro de precios si costo > 0.
 *
 * - Muestra o actualiza #precioLista, #precioContado, #precioFinal.
 * - En edición: muestra/oculta el contenedor #preciosNuevos según haya cambio.
 * - En creación: muestra #preciosCalculados si costo > 0.
 *
 * @param {Object|null} preciosActuales - { lista, contado, final } o null.
 */
window.calcularPrecios = function (preciosActuales = null) {
    const costo = parseFloat($('#PrecioCosto').val()) || 0;
    const margen = parseFloat($('#MargenVentaPct').val()) || 0;
    const descuento = parseFloat($('#DescuentoContadoPct').val()) || 0;
    const iva = parseFloat($('#IvaPct').val()) || 21;

    // Cálculos
    const precioLista = costo * (1 + margen / 100);
    const precioContado = precioLista * (1 - descuento / 100);
    const precioFinal = precioContado * (1 + iva / 100);

    // Actualizo los spans correspondientes
    $('#precioLista').text(precioLista.toFixed(2));
    $('#precioContado').text(precioContado.toFixed(2));
    $('#precioFinal').text(precioFinal.toFixed(2));

    if (preciosActuales) {
        // MODO EDICIÓN: comparar contra valores actuales
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
        // MODO CREACIÓN: si hay costo > 0, muestro el recuadro de “precios calculados”
        if (costo > 0) {
            $('#preciosCalculados').show();
        } else {
            $('#preciosCalculados').hide();
        }
    }
};
