// wwwroot/js/utils/common.js
window.Utils = {
    // Funciones de Cliente
    cargarCiudades: function (provinciaId, selectorCiudad, ciudadSeleccionada = null) {
        const $ciudadSelect = $(selectorCiudad);
        $ciudadSelect.empty().append('<option value="">-- Seleccione Ciudad --</option>');

        if (!provinciaId) {
            return;
        }

        $.ajax({
            url: '/Cliente/GetCiudades',
            type: 'GET',
            data: { provinciaId: provinciaId },
            success: function (ciudades) {
                ciudades.forEach(function (ciudad) {
                    const $opt = $('<option></option>')
                        .attr('value', ciudad.value)
                        .text(ciudad.text)
                        .data('provincia-id', ciudad.provinciaId);

                    if (ciudadSeleccionada && ciudad.value == ciudadSeleccionada) {
                        $opt.attr('selected', 'selected');
                    }
                    $ciudadSelect.append($opt);
                });
            },
            error: function () {
                console.error('Error al cargar ciudades para provinciaId=' + provinciaId);
            }
        });
    },

    validarDni: function (selectorInput, dniOriginal = null) {
        const $input = $(selectorInput);
        const valor = $input.val().trim();

        if (!valor) {
            $input.removeClass('is-invalid');
            $input.next('.text-danger').text('');
            return;
        }

        if (dniOriginal && valor === String(dniOriginal)) {
            $input.removeClass('is-invalid');
            $input.next('.text-danger').text('');
            return;
        }

        const patron = /^\d{7,8}$/;
        if (!patron.test(valor)) {
            $input.addClass('is-invalid');
            $input.next('.text-danger').text('El DNI debe tener 7 u 8 dígitos');
        } else {
            $input.removeClass('is-invalid');
            $input.next('.text-danger').text('');
        }
    },

    // Funciones de Producto
    cargarRubros: function (selectorRubro) {
        console.log('cargarRubros: Cargando...');
        var select = $(selectorRubro);
        select.empty().append('<option value="">-- Seleccione --</option>');

        $.get('/Producto/GetRubros')
            .done(function (rubros) {
                console.log('Rubros cargados:', rubros);
                rubros.forEach(r => {
                    select.append($('<option></option>').val(r.value).text(r.text));
                });
            })
            .fail(function (error) {
                console.error('Error cargando rubros:', error);
            });
    },

    cargarSubrubros: function (rubroId, selector, subrubroSeleccionado = null) {
        console.log('cargarSubrubros: rubroId=', rubroId);
        var select = $(selector);
        select.empty().append('<option value="">-- Seleccione --</option>');

        if (!rubroId) return;

        $.get('/Producto/GetSubrubros', { rubroId: rubroId })
            .done(function (subrubros) {
                console.log('Subrubros cargados:', subrubros);
                subrubros.forEach(s => {
                    const $opt = $('<option></option>').val(s.value).text(s.text);
                    if (subrubroSeleccionado && s.value == subrubroSeleccionado) {
                        $opt.attr('selected', 'selected');
                    }
                    select.append($opt);
                });
            })
            .fail(function (error) {
                console.error('Error cargando subrubros:', error);
            });
    },

    cargarSubmarcas: function (marcaId, selectorSubmarca, submarcaSeleccionada = null) {
        const $submarcaSelect = $(selectorSubmarca);
        $submarcaSelect.empty().append('<option value="">-- Seleccione --</option>');

        if (!marcaId) return;

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
    },

    cargarMarcas: function (selectorMarca) {
        console.log('cargarMarcas: Cargando...');
        var select = $(selectorMarca);
        select.empty().append('<option value="">-- Seleccione --</option>');

        $.get('/Producto/GetMarcas')
            .done(function (marcas) {
                console.log('Marcas cargadas:', marcas);
                marcas.forEach(m => {
                    select.append($('<option></option>').val(m.value).text(m.text));
                });
            })
            .fail(function (error) {
                console.error('Error cargando marcas:', error);
            });
    },

    calcularPrecios: function (preciosActuales = null) {
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

            $('#preciosNuevos').toggle(cambio);
        } else {
            $('#preciosCalculados').toggle(costo > 0);
        }
    }
};

// Función global para abrir modales
window.abrirModal = function (tipo) {
    console.log('Abriendo modal para:', tipo);

    switch (tipo) {
        case 'Rubro':
            $('#modalCategoria').modal('show');
            $('#categoriaTipo').val('R');
            $('#divParent').hide();
            $('#categoriaNombre').val('');
            break;
        case 'Subrubro':
            $('#modalCategoria').modal('show');
            $('#categoriaTipo').val('S');
            $('#divParent').show();
            $('#categoriaNombre').val('');
            break;
        case 'Marca':
            $('#modalMarca').modal('show');
            $('#marcaTipo').val('M');
            $('#divMarcaParent').hide();
            $('#marcaNombre').val('');
            break;
        case 'Submarca':
            $('#modalMarca').modal('show');
            $('#marcaTipo').val('S');
            $('#divMarcaParent').show();
            $('#marcaNombre').val('');
            break;
    }
};