function initClienteEdit(ciudadActual, dniOriginal) {
    $('#provinciaSelect').on('change', function () {
        var provinciaId = $(this).val();
        cargarCiudades(provinciaId, '#CiudadId', ciudadActual);
    });

    if (ciudadActual > 0) {
        var provinciaId = $('#CiudadId option:selected').data('provincia-id');
        if (provinciaId) {
            $('#provinciaSelect').val(provinciaId).trigger('change');
        }
    }

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

    $('#Dni').on('blur', function () {
        validarDni('#Dni', dniOriginal);
    });
}