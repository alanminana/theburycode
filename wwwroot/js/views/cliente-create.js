function initClienteCreate() {
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
                        ciudadSelect.append($('<option></option>')
                            .attr('value', ciudad.value)
                            .text(ciudad.text));
                    });
                }
            });
        }
    });

    // Validación de DNI al salir del campo
    $('#Dni').on('blur', function () {
        var dni = $(this).val();
        if (dni) {
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
