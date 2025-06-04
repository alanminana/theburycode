function initClienteCreate() {
    $('#provinciaSelect').on('change', function () {
        var provinciaId = $(this).val();
        cargarCiudades(provinciaId, '#CiudadId');
    });

    $('#Dni').on('blur', function () {
        validarDni('#Dni');
    });
}