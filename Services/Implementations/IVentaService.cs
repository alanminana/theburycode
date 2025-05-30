
// Services/Interfaces/IVentaService.cs
using theburycode.Models;

namespace theburycode.Services
{
    public interface IVentaService
    {
        Task<List<Venta>> GetAllAsync();
        Task<Venta> GetByIdAsync(int id);
        Task<List<Venta>> GetByClienteAsync(int clienteId);
        Task<List<Venta>> GetByFechaAsync(DateTime desde, DateTime hasta);
        Task<Venta> CreateAsync(Venta venta, List<VentaDetalle> detalles);
        Task<bool> CancelarAsync(int id);
        Task<string> GenerarNumeroFacturaAsync();
        Task<bool> ActualizarEstadoEntregaAsync(int id, string nuevoEstado);
    }
}