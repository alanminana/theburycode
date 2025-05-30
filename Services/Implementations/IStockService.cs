using theburycode.Models;

using theburycode.Services.Shared;

namespace theburycode.Services.Shared
{
    public interface IStockService
    {
        Task<bool> AjustarStock(int productoId, int cantidad, string tipoMovimiento, string origen, string usuario);
        Task<bool> ValidarStockDisponible(int productoId, int cantidadRequerida);
        Task<List<StockLog>> GetHistorialStock(int productoId, DateTime? desde = null, DateTime? hasta = null);
        Task<List<Producto>> GetProductosStockBajo();
        Task<bool> TransferirStock(int productoOrigenId, int productoDestinoId, int cantidad, string usuario);
    }
}