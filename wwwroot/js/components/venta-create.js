// wwwroot/js/components/venta-create.js
// Ejemplo de uso con el nuevo sistema de componentes

const ventaApp = createVueApp({
    data() {
        return {
            venta: {
                clienteId: null,
                formaPago: {},
                detalles: []
            },
            cliente: null,
            subtotal: 0,
            iva: 0,
            total: 0
        }
    },
    computed: {
        puedeGuardar() {
            return this.venta.clienteId && this.venta.detalles.length > 0;
        }
    },
    methods: {
        onClienteSeleccionado(cliente) {
            this.cliente = cliente;
            this.venta.clienteId = cliente.id;
        },
        onTotalChanged(total) {
            this.subtotal = total;
            this.iva = total * 0.21;
            this.total = total * 1.21;
        },
        onFormaPagoChanged(formaPago) {
            this.venta.formaPago = formaPago;
        },
        formatPrice(value) {
            return window.formatPrice(value);
        },
        async guardarVenta() {
            if (!this.puedeGuardar) {
                Swal.fire('Error', 'Complete todos los campos requeridos', 'error');
                return;
            }

            try {
                const response = await axios.post('/api/ventas', this.venta);
                if (response.data.success) {
                    Swal.fire('Éxito', 'Venta guardada correctamente', 'success')
                        .then(() => {
                            window.location.href = '/Venta';
                        });
                }
            } catch (error) {
                Swal.fire('Error', error.response?.data?.message || 'Error al guardar la venta', 'error');
            }
        }
    }
});

// Montar la aplicación
ventaApp.mount('#ventaApp');