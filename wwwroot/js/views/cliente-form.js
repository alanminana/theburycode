// wwwroot/js/views/cliente-form.js
window.ClienteForm = {
    init: function (options = {}) {
        const { ciudadActual, dniOriginal, modo = 'create' } = options;

        // Manejo de provincia/ciudad
        $('#provinciaSelect').on('change', function () {
            var provinciaId = $(this).val();
            Utils.cargarCiudades(provinciaId, '#CiudadId', ciudadActual);
        });

        // Para edición, cargar la provincia actual
        if (modo === 'edit' && ciudadActual > 0) {
            var provinciaId = $('#CiudadId option:selected').data('provincia-id');
            if (provinciaId) {
                $('#provinciaSelect').val(provinciaId).trigger('change');
            }
        }

        // Validación de DNI
        $('#Dni').on('blur', function () {
            Utils.validarDni('#Dni', dniOriginal);
        });

        // Scoring display (solo en edición)
        if (modo === 'edit') {
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
        }

        // Formulario principal
        $('#formCliente').on('submit', function (e) {
            // Validación adicional si es necesaria
            if (!$('#Dni').val() || !$('#Nombre').val() || !$('#Apellido').val()) {
                e.preventDefault();
                Swal.fire('Error', 'Complete todos los campos requeridos', 'error');
                return false;
            }
        });

        console.log(`ClienteForm inicializado en modo: ${modo}`);
    }
};