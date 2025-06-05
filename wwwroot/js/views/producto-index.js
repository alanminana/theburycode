function initProductoIndex() {
    console.log('initProductoIndex: Inicializando');

    $('#checkAll').on('change', function () {
        const isChecked = $(this).prop('checked');
        console.log('checkAll cambiado:', isChecked);
        $('.checkProducto').prop('checked', isChecked);
    });

    window.mostrarAjusteMasivo = function () {
        const seleccionados = $('.checkProducto:checked').length;
        console.log('mostrarAjusteMasivo: productos seleccionados:', seleccionados);

        if (seleccionados === 0) {
            Swal.fire('Atención', 'Debe seleccionar al menos un producto', 'warning');
            return;
        }
        $('#modalAjusteMasivo').modal('show');
    }

    window.aplicarAjusteMasivo = async function () {
        const productosIds = [];
        $('.checkProducto:checked').each(function () {
            const id = parseInt($(this).val());
            console.log('Agregando producto ID:', id);
            productosIds.push(id);
        });

        const tipoAjuste = $('#tipoAjuste').val();
        const porcentaje = parseFloat($('#porcentajeAjuste').val());

        console.log('aplicarAjusteMasivo - datos:', {
            productosIds: productosIds,
            tipoAjuste: tipoAjuste,
            porcentaje: porcentaje
        });

        if (porcentaje <= 0) {
            Swal.fire('Error', 'El porcentaje debe ser mayor a 0', 'error');
            return;
        }

        try {
            const response = await $.ajax({
                url: '/Producto/AjustePrecioMasivo',
                type: 'POST',
                data: {
                    productosIds: productosIds,
                    porcentaje: porcentaje,
                    tipoAjuste: tipoAjuste
                },
                traditional: true
            });

            console.log('Respuesta ajuste masivo:', response);

            if (response.success) {
                $('#modalAjusteMasivo').modal('hide');
                Swal.fire({
                    icon: 'success',
                    title: 'Ajuste aplicado',
                    text: `Se actualizaron ${response.actualizados} productos`,
                    confirmButtonText: 'OK'
                }).then(() => {
                    location.reload();
                });
            } else {
                console.error('Error en respuesta:', response.error);
                Swal.fire('Error', response.error, 'error');
            }
        } catch (error) {
            console.error('Error AJAX:', error);
            Swal.fire('Error', 'Error al aplicar el ajuste', 'error');
        }
    }
}