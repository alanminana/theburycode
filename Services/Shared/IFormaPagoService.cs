// Services/Shared/IFormaPagoService.cs
using theburycode.Models;

namespace theburycode.Services.Shared
{
    public interface IFormaPagoService
    {
        Task<List<FormaPago>> GetFormasPagoActivas();
        Task<List<Banco>> GetBancosActivos();
        Task<List<TipoTarjeta>> GetTiposTarjeta();
        Task<bool> RequiereDatosBancarios(int formaPagoId);
        Task<FormaPagoDto> GetDatosFormaPago(int formaPagoId);
    }
}