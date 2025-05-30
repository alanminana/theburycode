// wwwroot/js/views/venta-create.js
const { createApp } = Vue;

createApp({
    data() {
        return {
            venta: {
                clienteId: null,
                formaPago: {},
                detalles: [],
                condiciones: ''
            },
            cliente: null,
            subtotal: 0,
            iva: 0,
            total: 0
        }
    },
    computed: {
        puedeGuardar() {
            return this.venta.clienteId &&
                this.venta.formaPago.formaPagoId &&
                this.venta.detalles.length > 0;
        }
    },
    methods: {
        onClienteSeleccionado(cliente) {
            this.cliente = cliente;
            this.verificarCredito();
        },
        onTotalChanged(nuevoTotal) {
            this.subtotal = nuevoTotal;
            this.iva = nuevoTotal * 0.21;
            this.total = nuevoTotal * 1.21;
        },
        async verificarCredito() {
            if (this.cliente.tieneDeuda) {
                await Swal.fire({
                    icon: 'warning',
                    title: 'Cliente con deuda',
                    text: 'Este cliente tiene deudas pendientes',
                    confirmButtonText: 'Entendido'
                });
            }
        },
        async guardarVenta() {
            try {
                const response = await axios.post('/api/ventas', {
                    clienteId: this.venta.clienteId,
                    formaPagoId: this.venta.formaPago.formaPagoId,
                    bancoId: this.venta.formaPago.bancoId,
                    tipoTarjetaId: this.venta.formaPago.tipoTarjetaId,
                    condiciones: this.venta.condiciones,
                    detalles: this.venta.detalles.map(d => ({
                        productoId: d.productoId,
                        cantidad: d.cantidad,
                        precioUnit: d.precioUnit
                    }))
                });

                await Swal.fire({
                    icon: 'success',
                    title: 'Venta guardada',
                    text: `Factura: ${response.data.numeroFactura}`
                });

                window.location.href = '/Venta';
            } catch (error) {
                console.error('Error:', error);
                await Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: error.response?.data?.message || 'Error al guardar la venta'
                });
            }
        },
        formatPrice(value) {
            return new Intl.NumberFormat('es-AR', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }).format(value);
        }
    },
    components: {
        'cliente-search': ClienteSearch,
        'forma-pago-selector': FormaPagoSelector,
        'detalle-grid': DetalleGrid
    }
}).mount('#ventaApp');

[ApiController]
[Route("api/search")]
public class SearchApiController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchApiController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("clientes")]
    public async Task < IActionResult > BuscarClientes(string termino)
    {
        var clientes = await _searchService.BuscarClientes(termino);
        return Ok(clientes);
    }

    [HttpGet("productos")]
    public async Task < IActionResult > BuscarProductos(string termino)
    {
        var productos = await _searchService.BuscarProductos(termino);
        return Ok(productos);
    }
}