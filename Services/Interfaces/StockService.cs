
// Services/Interfaces/IVentaService.cs
using Microsoft.EntityFrameworkCore;
using theburycode.Models;
using theburycode.Services.Shared;

namespace theburycode.Services
{
    public class StockService : IStockService
    {
        private readonly TheBuryCodeContext _context;
        private readonly ILogger<StockService> _logger;

        public StockService(TheBuryCodeContext context, ILogger<StockService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AjustarStock(int productoId, int cantidad, string tipoMovimiento, string origen, string usuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var producto = await _context.Productos.FindAsync(productoId);
                if (producto == null) return false;

                var stockAnterior = producto.StockActual ?? 0;
                var stockNuevo = stockAnterior + cantidad;

                if (stockNuevo < 0)
                {
                    _logger.LogWarning($"Stock insuficiente para producto {productoId}. Stock actual: {stockAnterior}, solicitado: {cantidad}");
                    return false;
                }

                producto.StockActual = stockNuevo;
                producto.FechaModificacion = DateTime.Now;
                producto.UsuarioModificacion = usuario;

                // Crear log
                var log = new StockLog
                {
                    ProductoId = productoId,
                    FechaMovimiento = DateTime.Now,
                    CantidadAnterior = stockAnterior,
                    CantidadNueva = stockNuevo,
                    TipoMovimiento = tipoMovimiento,
                    Origen = origen,
                    Usuario = usuario
                };

                _context.StockLogs.Add(log);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Stock ajustado: Producto {productoId}, {stockAnterior} -> {stockNuevo}, Tipo: {tipoMovimiento}");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al ajustar stock");
                throw;
            }
        }

        public async Task<bool> ValidarStockDisponible(int productoId, int cantidadRequerida)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            return producto != null && (producto.StockActual ?? 0) >= cantidadRequerida;
        }

        public async Task<List<StockLog>> GetHistorialStock(int productoId, DateTime? desde = null, DateTime? hasta = null)
        {
            var query = _context.StockLogs.Where(s => s.ProductoId == productoId);

            if (desde.HasValue)
                query = query.Where(s => s.FechaMovimiento >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(s => s.FechaMovimiento <= hasta.Value);

            return await query.OrderByDescending(s => s.FechaMovimiento).ToListAsync();
        }

        public async Task<List<Producto>> GetProductosStockBajo()
        {
            return await _context.Productos
                .Where(p => p.Activo == true && p.StockActual < p.StockMinimo)
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .ToListAsync();
        }

        public async Task<bool> TransferirStock(int productoOrigenId, int productoDestinoId, int cantidad, string usuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Reducir del origen
                var resultOrigen = await AjustarStock(productoOrigenId, -cantidad, "TRANSFERENCIA_SALIDA",
                    $"Transferencia a producto {productoDestinoId}", usuario);

                if (!resultOrigen)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // Aumentar en destino
                await AjustarStock(productoDestinoId, cantidad, "TRANSFERENCIA_ENTRADA",
                    $"Transferencia desde producto {productoOrigenId}", usuario);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}