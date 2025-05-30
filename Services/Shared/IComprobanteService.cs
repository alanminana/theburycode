// Services/Shared/IComprobanteService.cs
using theburycode.Models;

namespace theburycode.Services.Shared
{
    public interface IComprobanteService
    {
        Task<string> GenerarNumero(string prefijo);
        Task<string> GenerarNumeroFactura();
        Task<string> GenerarNumeroCotizacion();
        Task<string> GenerarNumeroCompra();
        Task<string> GenerarNumeroRMA();
        bool ValidarFormato(string numero, string prefijo);
    }
}
