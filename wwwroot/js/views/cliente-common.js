// cliente-common.js

window.cargarCiudades = function (provinciaId, selectorCiudad, ciudadSeleccionada = null) {
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
};

window.validarDni = function (selectorInput, dniOriginal = null) {
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
};