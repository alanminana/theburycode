using Microsoft.EntityFrameworkCore;
using theburycode.Models;

namespace theburycode.Services
{
    public class ProductoService : IProductoService
    {
        private readonly TheBuryCodeContext _context;
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(TheBuryCodeContext context, ILogger<ProductoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Producto>> GetAllAsync()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Submarca)
                .Where(p => p.Activo == true)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Submarca)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Producto>> GetByCategoriaAsync(int categoriaId)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.CategoriaId == categoriaId && p.Activo == true)
                .ToListAsync();
        }

        public async Task<List<Producto>> GetByMarcaAsync(int marcaId)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.MarcaId == marcaId && p.Activo == true)
                .ToListAsync();
        }

        public async Task<List<Producto>> GetStockBajoAsync()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.StockActual < p.StockMinimo && p.Activo == true)
                .ToListAsync();
        }

        public async Task<Producto> CreateAsync(Producto producto)
        {
            producto.FechaAlta = DateTime.Now;
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Producto creado: {producto.Id} - {producto.Nombre}");
            return producto;
        }

        public async Task<Producto> UpdateAsync(Producto producto)
        {
            var original = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == producto.Id);

            // Log cambio de precio si cambió
            if (original != null && original.PrecioCosto != producto.PrecioCosto)
            {
                var precioLog = new PrecioLog
                {
                    ProductoId = producto.Id,
                    PrecioAnterior = original.PrecioCosto,
                    PrecioNuevo = producto.PrecioCosto,
                    FechaCambio = DateTime.Now,
                    Usuario = producto.UsuarioModificacion
                };
                _context.PrecioLogs.Add(precioLog);
                _logger.LogInformation($"Precio cambiado: Producto {producto.Id}, de ${original.PrecioCosto} a ${producto.PrecioCosto}");
            }

            // Log cambio de stock si cambió
            if (original != null && original.StockActual != producto.StockActual)
            {
                var stockLog = new StockLog
                {
                    ProductoId = producto.Id,
                    CantidadAnterior = original.StockActual,
                    CantidadNueva = producto.StockActual,
                    TipoMovimiento = "AJUSTE_MANUAL",
                    FechaMovimiento = DateTime.Now,
                    Usuario = producto.UsuarioModificacion,
                    Origen = "Edición manual"
                };
                _context.StockLogs.Add(stockLog);
                _logger.LogInformation($"Stock cambiado: Producto {producto.Id}, de {original.StockActual} a {producto.StockActual}");
            }

            producto.FechaModificacion = DateTime.Now;
            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return producto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                _logger.LogWarning($"Producto {id} no encontrado para eliminar");
                return false;
            }

            // Verificar si tiene movimientos
            var tieneVentas = await _context.VentaDetalles.AnyAsync(v => v.ProductoId == id);
            var tieneCompras = await _context.CompraDetalles.AnyAsync(c => c.ProductoId == id);
            var tieneCotizaciones = await _context.CotizacionDetalles.AnyAsync(c => c.ProductoId == id);

            if (tieneVentas || tieneCompras || tieneCotizaciones)
            {
                // Desactivar en lugar de eliminar físicamente
                producto.Activo = false;
                producto.EstadoProducto = "I";
                producto.FechaModificacion = DateTime.Now;
                producto.UsuarioModificacion = "SYSTEM";
                producto.FechaSuspension = DateTime.Now;

                _logger.LogInformation($"Producto {id} desactivado por tener movimientos asociados");
            }
            else
            {
                // Eliminar físicamente si no tiene movimientos
                _context.Productos.Remove(producto);
                _logger.LogInformation($"Producto {id} eliminado físicamente");
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStockAsync(int productoId, int cantidad, string tipoMovimiento)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return false;

            var stockAnterior = producto.StockActual ?? 0;
            producto.StockActual = stockAnterior + cantidad;

            // Log de movimiento de stock
            var stockLog = new StockLog
            {
                ProductoId = productoId,
                CantidadAnterior = stockAnterior,
                CantidadNueva = producto.StockActual,
                TipoMovimiento = tipoMovimiento,
                FechaMovimiento = DateTime.Now,
                Usuario = "SYSTEM"
            };
            _context.StockLogs.Add(stockLog);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Stock actualizado: Producto {productoId}, {tipoMovimiento}, cantidad: {cantidad}");
            return true;
        }

        public async Task<decimal> CalcularPrecioListaAsync(int productoId)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return 0;

            var margen = producto.MargenVentaPct ?? 0;
            return producto.PrecioCosto.GetValueOrDefault() * (1 + margen / 100);
        }

        public async Task<decimal> CalcularPrecioContadoAsync(int productoId)
        {
            var precioLista = await CalcularPrecioListaAsync(productoId);
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return 0;

            var descuento = producto.DescuentoContadoPct ?? 0;
            return precioLista * (1 - descuento / 100);
        }
    }
}