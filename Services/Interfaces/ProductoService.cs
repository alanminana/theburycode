
// Services/Implementations/ProductoService.cs
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
                .Where(p => p.Activo == true)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<Producto> GetByIdAsync(int id)
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
            if (original.PrecioCosto != producto.PrecioCosto)
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
            }

            producto.FechaModificacion = DateTime.Now;
            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return producto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return false;

            // Soft delete
            producto.Activo = false;
            producto.FechaModificacion = DateTime.Now;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Producto desactivado: {id}");
            return true;
        }

        public async Task<bool> UpdateStockAsync(int productoId, int cantidad, string tipoMovimiento)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return false;

            var stockAnterior = producto.StockActual;
            producto.StockActual += cantidad;

            // Log de movimiento de stock
            var stockLog = new StockLog
            {
                ProductoId = productoId,
                CantidadAnterior = stockAnterior,
                CantidadNueva = producto.StockActual,
                TipoMovimiento = tipoMovimiento,
                FechaMovimiento = DateTime.Now
            };
            _context.StockLogs.Add(stockLog);

            await _context.SaveChangesAsync();
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

            var descuento = producto.DescuentoContadoPct ?? 0;
            return precioLista * (1 - descuento / 100);
        }
    }
}