// Services/Shared/IFormaPagoService.cs
using theburycode.Models;

namespace theburycode.Services.Shared
{
    public interface IFormaPagoService
    {
        Task<List<FormaPago>> GetFormasPagoActivas();
        Task<List<Banco>> GetBancosActivos();
        Task<List<TipoTarjetum>> GetTiposTarjeta(); // Cambiar TipoTarjeta por TipoTarjetum
        Task<bool> RequiereDatosBancarios(int formaPagoId);
        Task<FormaPagoDto> GetDatosFormaPago(int formaPagoId);
    }

    public class FormaPagoDto
    {
        public int Id { get; set; }
        public required string Descripcion { get; set; } // Agregar required
        public bool RequiereBanco { get; set; }
        public bool RequiereTarjeta { get; set; }
        public bool PermiteCuotas { get; set; }
        public int MaximoCuotas { get; set; }
    }
}