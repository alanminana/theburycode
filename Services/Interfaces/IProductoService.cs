
// Services/Interfaces/IProductoService.cs
using theburycode.Models;

namespace theburycode.Services
{
    public interface IProductoService
    {
        Task<List<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);

        Task<List<Producto>> GetByCategoriaAsync(int categoriaId);
        Task<List<Producto>> GetByMarcaAsync(int marcaId);
        Task<List<Producto>> GetStockBajoAsync();
        Task<Producto> CreateAsync(Producto producto);
        Task<Producto> UpdateAsync(Producto producto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStockAsync(int productoId, int cantidad, string tipoMovimiento);
        Task<decimal> CalcularPrecioListaAsync(int productoId);
        Task<decimal> CalcularPrecioContadoAsync(int productoId);
    }
}
