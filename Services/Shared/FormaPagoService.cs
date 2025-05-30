// Services/Shared/IFormaPagoService.cs
using Microsoft.EntityFrameworkCore;
using theburycode.Models;

namespace theburycode.Services.Shared
{

    public class FormaPagoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool RequiereBanco { get; set; }
        public bool RequiereTarjeta { get; set; }
        public bool PermiteCuotas { get; set; }
        public int? MaximoCuotas { get; set; }
    }

    public class FormaPagoService : IFormaPagoService
    {
        private readonly TheBuryCodeContext _context;

        public FormaPagoService(TheBuryCodeContext context)
        {
            _context = context;
        }

        public async Task<List<FormaPago>> GetFormasPagoActivas()
        {
            return await _context.FormaPagos.ToListAsync();
        }

        public async Task<List<Banco>> GetBancosActivos()
        {
            return await _context.Bancos.ToListAsync();
        }

        public async Task<List<TipoTarjeta>> GetTiposTarjeta()
        {
            return await _context.TipoTarjeta.ToListAsync();
        }

        public async Task<bool> RequiereDatosBancarios(int formaPagoId)
        {
            var formaPago = await _context.FormaPagos.FindAsync(formaPagoId);
            if (formaPago == null) return false;

            // Lógica según descripción
            return formaPago.Descripcion.Contains("Tarjeta") ||
                   formaPago.Descripcion.Contains("Transferencia");
        }

        public async Task<FormaPagoDto> GetDatosFormaPago(int formaPagoId)
        {
            var formaPago = await _context.FormaPagos.FindAsync(formaPagoId);
            if (formaPago == null) return null;

            return new FormaPagoDto
            {
                Id = formaPago.Id,
                Descripcion = formaPago.Descripcion,
                RequiereBanco = formaPago.Descripcion.Contains("Tarjeta") || formaPago.Descripcion.Contains("Transferencia"),
                RequiereTarjeta = formaPago.Descripcion.Contains("Tarjeta"),
                PermiteCuotas = formaPago.Descripcion.Contains("Crédito"),
                MaximoCuotas = formaPago.Descripcion.Contains("Crédito") ? 12 : 1
            };
        }
    }
}