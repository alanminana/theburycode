function initClienteEdit(ciudadActual, dniOriginal) {
    // Guardar la ciudad actual
    if (ciudadActual > 0) {
        var provinciaId = $('#CiudadId option:selected').data('provincia-id');
        if (provinciaId) {
            $('#provinciaSelect').val(provinciaId).trigger('change');
        }
    }

    // Cargar ciudades cuando se selecciona provincia
    $('#provinciaSelect').on('change', function () {
        var provinciaId = $(this).val();
        var ciudadSelect = $('#CiudadId');

        ciudadSelect.empty();
        ciudadSelect.append('<option value="">-- Seleccione Ciudad --</option>');

        if (provinciaId) {
            $.ajax({
                url: '/Cliente/GetCiudades',
                type: 'GET',
                data: { provinciaId: provinciaId },
                success: function (ciudades) {
                    $.each(ciudades, function (index, ciudad) {
                        var option = $('<option></option>')
                            .attr('value', ciudad.value)
                            .text(ciudad.text);

                        if (ciudad.value == ciudadActual) {
                            option.attr('selected', 'selected');
                        }

                        ciudadSelect.append(option);
                    });
                }
            });
        }
    });

    // Actualizar visualización de scoring
    $('#Scoring').on('input', function () {
        var valor = $(this).val();
        var display = $('#scoringDisplay');

        if (valor >= 1 && valor <= 10) {
            var clase = valor >= 8 ? 'text-success' : valor >= 5 ? 'text-warning' : 'text-danger';
            display.html('<span class="' + clase + '"><i class="fas fa-star"></i> ' + valor + '</span>');
        } else {
            display.html('');
        }
    });

    // Validación de DNI
    $('#Dni').on('blur', function () {
        var dni = $(this).val();
        if (dni && dni !== dniOriginal) {
            if (!/^\d{7,8}$/.test(dni)) {
                $(this).addClass('is-invalid');
                $(this).next('.text-danger').text('El DNI debe tener 7 u 8 dígitos');
            } else {
                $(this).removeClass('is-invalid');
                $(this).next('.text-danger').text('');
            }
        }
    });
}
