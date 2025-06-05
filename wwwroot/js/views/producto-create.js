function initProductoCreate() {
    console.log('initProductoCreate: Inicializando');

    cargarRubros();

    $('#rubroSelect').on('change', function () {
        var rubroId = $(this).val();
        console.log('Rubro seleccionado:', rubroId);
        cargarSubrubros(rubroId, '#CategoriaId');
    });

    $('#MarcaId').on('change', function () {
        var marcaId = $(this).val();
        console.log('Marca seleccionada:', marcaId);
        cargarSubmarcas(marcaId, '#SubmarcaId');
    });

    $('#PrecioCosto, #MargenVentaPct, #DescuentoContadoPct, #IvaPct').on('input', function () {
        console.log('Cambio en precio:', $(this).attr('id'), '=', $(this).val());
        calcularPrecios();
    });
}

function cargarRubros() {
    console.log('cargarRubros: Cargando...');
    $.get('/Producto/GetRubros')
        .done(function (rubros) {
            console.log('Rubros cargados:', rubros);
            var select = $('#rubroSelect');
            select.empty().append('<option value="">-- Seleccione --</option>');
            rubros.forEach(r => {
                select.append($('<option>').val(r.value).text(r.text));
            });
        })
        .fail(function (error) {
            console.error('Error cargando rubros:', error);
        });
}

function cargarSubrubros(rubroId, selector) {
    console.log('cargarSubrubros: rubroId=', rubroId);
    var select = $(selector);
    select.empty().append('<option value="">-- Seleccione --</option>');

    if (rubroId) {
        $.get('/Producto/GetSubrubros', { rubroId: rubroId })
            .done(function (subrubros) {
                console.log('Subrubros cargados:', subrubros);
                subrubros.forEach(s => {
                    select.append($('<option>').val(s.value).text(s.text));
                });
            })
            .fail(function (error) {
                console.error('Error cargando subrubros:', error);
            });
    }
}