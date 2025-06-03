
// Services/Implementations/VentaService.cs
using Microsoft.EntityFrameworkCore;
using theburycode.Models;

namespace theburycode.Services
{
    public class VentaService : IVentaService
    {
        private readonly TheBuryCodeContext _context;
        private readonly IProductoService _productoService;
        private readonly ILogger<VentaService> _logger;

        public VentaService(TheBuryCodeContext context, IProductoService productoService, ILogger<VentaService> logger)
        {
            _context = context;
            _productoService = productoService;
            _logger = logger;
        }

        public async Task<List<Venta>> GetAllAsync()
        {
            return await _context.Venta
                .Include(v => v.Cliente)
                .Include(v => v.FormaPago)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        public async Task<Venta?> GetByIdAsync(int id)
        {
            return await _context.Venta
                .Include(v => v.Cliente)
                .Include(v => v.FormaPago)
                .Include(v => v.VentaDetalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Venta>> GetByClienteAsync(int clienteId)
        {
            return await _context.Venta
                .Include(v => v.FormaPago)
                .Where(v => v.ClienteId == clienteId)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        public async Task<List<Venta>> GetByFechaAsync(DateTime desde, DateTime hasta)
        {
            return await _context.Venta
                .Include(v => v.Cliente)
                .Include(v => v.FormaPago)
                .Where(v => v.Fecha >= desde && v.Fecha <= hasta)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        public async Task<Venta> CreateAsync(Venta venta, List<VentaDetalle> detalles)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Generar número de factura
                venta.NumeroFactura = await GenerarNumeroFacturaAsync();
                venta.FechaAlta = DateTime.Now;

                // Calcular total
                decimal total = 0;
                foreach (var detalle in detalles)
                {
                    total += detalle.Cantidad * detalle.PrecioUnit;

                    // Actualizar stock
                    await _productoService.UpdateStockAsync(detalle.ProductoId, -detalle.Cantidad, "VENTA");
                }
                venta.ImporteTotal = total;

                _context.Venta.Add(venta);
                await _context.SaveChangesAsync();

                // Agregar detalles
                foreach (var detalle in detalles)
                {
                    detalle.VentaId = venta.Id;
                    detalle.FechaAlta = DateTime.Now;
                    detalle.UsuarioAlta = venta.UsuarioAlta;
                    _context.VentaDetalles.Add(detalle);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation($"Venta creada: {venta.NumeroFactura}");

                return venta;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al crear venta");
                throw;
            }
        }

        public async Task<bool> CancelarAsync(int id)
        {
            var venta = await GetByIdAsync(id);
            if (venta == null) return false;

            // Devolver stock
            foreach (var detalle in venta.VentaDetalles)
            {
                await _productoService.UpdateStockAsync(detalle.ProductoId, detalle.Cantidad, "CANCELACION_VENTA");
            }

            // Marcar como anulada (no eliminar físicamente)
            venta.EstadoEntrega = "A"; // Anulada
            venta.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerarNumeroFacturaAsync()
        {
            var fecha = DateTime.Now;
            var prefijo = $"FAC-{fecha:yyyyMMdd}-";

            var ultimaFactura = await _context.Venta
    .Where(v => v.NumeroFactura != null && v.NumeroFactura.StartsWith(prefijo))
       .OrderByDescending(v => v.NumeroFactura)
    .FirstOrDefaultAsync();

            if (ultimaFactura?.NumeroFactura == null)
            {
                return $"{prefijo}0001";
            }

            var ultimoNumero = int.Parse(ultimaFactura.NumeroFactura.Substring(prefijo.Length));
            return $"{prefijo}{(ultimoNumero + 1):D4}";
        }

        public async Task<bool> ActualizarEstadoEntregaAsync(int id, string nuevoEstado)
        {
            var venta = await _context.Venta.FindAsync(id);
            if (venta == null) return false;

            venta.EstadoEntrega = nuevoEstado;
            venta.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}