function initProductoIndex() {
    // Check all
    $('#checkAll').on('change', function () {
        $('.checkProducto').prop('checked', $(this).prop('checked'));
    });

    window.mostrarAjusteMasivo = function () {
        const seleccionados = $('.checkProducto:checked').length;
        if (seleccionados === 0) {
            Swal.fire('Atención', 'Debe seleccionar al menos un producto', 'warning');
            return;
        }
        $('#modalAjusteMasivo').modal('show');
    }

    window.aplicarAjusteMasivo = async function () {
        const productosIds = [];
        $('.checkProducto:checked').each(function () {
            productosIds.push(parseInt($(this).val()));
        });

        const tipoAjuste = $('#tipoAjuste').val();
        const porcentaje = parseFloat($('#porcentajeAjuste').val());

        if (porcentaje <= 0) {
            Swal.fire('Error', 'El porcentaje debe ser mayor a 0', 'error');
            return;
        }

        try {
            const response = await axios.post('/Producto/AjustePrecioMasivo', {
                productosIds: productosIds,
                porcentaje: porcentaje,
                tipoAjuste: tipoAjuste
            });

            if (response.data.success) {
                $('#modalAjusteMasivo').modal('hide');
                Swal.fire({
                    icon: 'success',
                    title: 'Ajuste aplicado',
                    text: `Se actualizaron ${response.data.actualizados} productos`,
                    confirmButtonText: 'OK'
                }).then(() => {
                    location.reload();
                });
            } else {
                Swal.fire('Error', response.data.error, 'error');
            }
        } catch (error) {
            Swal.fire('Error', 'Error al aplicar el ajuste', 'error');
        }
    }
}
