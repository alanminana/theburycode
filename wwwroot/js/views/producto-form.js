// wwwroot/js/views/producto-form.js
window.ProductoForm = {
    init: function (options = {}) {
        const {
            marcaActual,
            submarcaActual,
            categoriaActual,
            rubroActual,
            preciosActuales,
            modo = 'create'
        } = options;

        console.log(`ProductoForm inicializado en modo: ${modo}`, options);

        // Solo para creación: configurar eventos de rubros
        if (modo === 'create') {
            $('#rubroSelect').on('change', function () {
                var rubroId = $(this).val();
                console.log('Rubro seleccionado:', rubroId);
                Utils.cargarSubrubros(rubroId, '#CategoriaId');
            });

            // También configurar el evento de marca
            $('#MarcaId').on('change', function () {
                var marcaId = $(this).val();
                console.log('Marca seleccionada:', marcaId);
                Utils.cargarSubmarcas(marcaId, '#SubmarcaId');
            });
        }

        // Para edición: preseleccionar rubro y cargar subrubros
        if (modo === 'edit' && categoriaActual && rubroActual) {
            // Si existe el select de rubro (solo en algunos casos de edición)
            if ($('#rubroSelect').length) {
                $('#rubroSelect').val(rubroActual);
                Utils.cargarSubrubros(rubroActual, '#CategoriaId', categoriaActual);
            }

            // Configurar el evento de marca para edición también
            $('#MarcaId').on('change', function () {
                var marcaId = $(this).val();
                console.log('Marca seleccionada:', marcaId);
                Utils.cargarSubmarcas(marcaId, '#SubmarcaId');
            });
        }

        // Manejo de marca/submarca en edición
        if (modo === 'edit' && marcaActual > 0) {
            Utils.cargarSubmarcas(marcaActual, '#SubmarcaId', submarcaActual);
        }

        // Cálculo de precios
        $('#PrecioCosto, #MargenVentaPct, #DescuentoContadoPct, #IvaPct').on('input', function () {
            console.log('Cambio en precio:', $(this).attr('id'), '=', $(this).val());
            Utils.calcularPrecios(preciosActuales);
        });

        // Trigger inicial de cálculo
        if ($('#PrecioCosto').val() > 0) {
            Utils.calcularPrecios(preciosActuales);
        }

        // Validación del formulario
        $('#formProducto').on('submit', function (e) {
            const campos = {
                codigo: $('#CodigoNum').val(),
                nombre: $('#Nombre').val(),
                categoria: $('#CategoriaId').val(),
                marca: $('#MarcaId').val(),
                precio: $('#PrecioCosto').val()
            };

            if (!campos.codigo || !campos.nombre || !campos.categoria || !campos.marca || !campos.precio) {
                e.preventDefault();
                Swal.fire('Error', 'Complete todos los campos requeridos', 'error');
                return false;
            }

            if (parseFloat(campos.precio) <= 0) {
                e.preventDefault();
                Swal.fire('Error', 'El precio debe ser mayor a 0', 'error');
                return false;
            }
        });
    }
};