const { createApp } = Vue;

createApp({
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
        },
        onTotalChanged(total) {
            this.subtotal = total;
            this.iva = total * 0.21;
            this.total = total * 1.21;
        },
        formatPrice(value) {
            return window.formatPrice(value);
        },
        async guardarVenta() {
            try {
                const response = await axios.post('/api/ventas', this.venta);
                if (response.data.success) {
                    Swal.fire('Éxito', 'Venta guardada', 'success');
                    window.location.href = '/Venta';
                }
            } catch (error) {
                Swal.fire('Error', error.response?.data?.message || 'Error al guardar', 'error');
            }
        }
    },
    components: {
        'cliente-search': ClienteSearch,
        'producto-search': ProductoSearch,
        'forma-pago-selector': FormaPagoSelector,
        'detalle-grid': DetalleGrid
    }
}).mount('#ventaApp');