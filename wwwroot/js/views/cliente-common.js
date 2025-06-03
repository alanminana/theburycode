// cliente-common.js

/**
 * Carga las ciudades para una provincia dada.
 * @param {number|string} provinciaId             - ID de la provincia seleccionada.
 * @param {string} selectorCiudad                 - Selector jQuery del <select> de ciudades (por ej. "#CiudadId").
 * @param {number|string|null} ciudadSeleccionada - (Opcional) ID de la ciudad que queremos pre-seleccionar.
 */
window.cargarCiudades = function (provinciaId, selectorCiudad, ciudadSeleccionada = null) {
    const $ciudadSelect = $(selectorCiudad);
    // Limpio y agrego opción por defecto
    $ciudadSelect.empty().append('<option value="">-- Seleccione Ciudad --</option>');

    if (!provinciaId) {
        // Si no viene provinciaId, no hago más nada
        return;
    }

    $.ajax({
        url: '/Cliente/GetCiudades',
        type: 'GET',
        data: { provinciaId: provinciaId },
        success: function (ciudades) {
            // ciudades = [ { value: ..., text: ..., provinciaId: ... }, ... ]
            ciudades.forEach(function (ciudad) {
                const $opt = $('<option></option>')
                    .attr('value', ciudad.value)
                    .text(ciudad.text)
                    // Para facilitar la detección de provincia original en edición:
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


/**
 * Valida un DNI en un input dado.
 * Si se pasa `dniOriginal`, solo valida si el valor actual es distinto al original.
 * @param {string} selectorInput - Selector jQuery del campo DNI (por ej. "#Dni").
 * @param {string|null} dniOriginal - (Opcional) valor de DNI original; si es null, valida siempre.
 */
window.validarDni = function (selectorInput, dniOriginal = null) {
    const $input = $(selectorInput);
    const valor = $input.val().trim();

    // Si no hay valor, no muestro error (puede ser requerido en el servidor)
    if (!valor) {
        $input.removeClass('is-invalid');
        $input.next('.text-danger').text('');
        return;
    }
    // Si existe dniOriginal y coincide con valor actual, no vuelvo a validar
    if (dniOriginal && valor === String(dniOriginal)) {
        $input.removeClass('is-invalid');
        $input.next('.text-danger').text('');
        return;
    }
    // Validación: 7 u 8 dígitos numéricos
    const patron = /^\d{7,8}$/;
    if (!patron.test(valor)) {
        $input.addClass('is-invalid');
        $input.next('.text-danger').text('El DNI debe tener 7 u 8 dígitos');
    } else {
        $input.removeClass('is-invalid');
        $input.next('.text-danger').text('');
    }
};
