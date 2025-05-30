// Services/Shared/FormaPagoService.cs
using Microsoft.EntityFrameworkCore;
using theburycode.Models;

namespace theburycode.Services.Shared
{
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

        public async Task<List<TipoTarjetum>> GetTiposTarjeta()
        {
            return await _context.TipoTarjeta.ToListAsync();
        }

        public async Task<bool> RequiereDatosBancarios(int formaPagoId)
        {
            var formaPago = await _context.FormaPagos.FindAsync(formaPagoId);
            if (formaPago == null) return false;
            
            // Lógica según el tipo de forma de pago
            return formaPago.Descripcion.ToUpper().Contains("TARJETA") || 
                   formaPago.Descripcion.ToUpper().Contains("TRANSFERENCIA");
        }

        public async Task<FormaPagoDto?> GetDatosFormaPago(int formaPagoId)
        {
            var formaPago = await _context.FormaPagos.FindAsync(formaPagoId);
            if (formaPago == null) return null;

            var descripcion = formaPago.Descripcion.ToUpper();

            return new FormaPagoDto
            {
                Id = formaPago.Id,
                Descripcion = formaPago.Descripcion,
                RequiereBanco = descripcion.Contains("TARJETA") || descripcion.Contains("TRANSFERENCIA"),
                RequiereTarjeta = descripcion.Contains("TARJETA"),
                PermiteCuotas = descripcion.Contains("TARJETA") || descripcion.Contains("CREDITO"),
                MaximoCuotas = descripcion.Contains("TARJETA") ? 12 : 1
            };
        }
    }
}