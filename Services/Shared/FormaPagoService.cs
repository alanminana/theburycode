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
            return formaPago?.Descripcion.ToLower().Contains("tarjeta") ?? false;
        }

        public async Task<FormaPagoDto?> GetDatosFormaPago(int formaPagoId)
        {
            var formaPago = await _context.FormaPagos.FindAsync(formaPagoId);
            if (formaPago == null) return null;

            var requiereBanco = formaPago.Descripcion.ToLower().Contains("tarjeta") ||
                               formaPago.Descripcion.ToLower().Contains("transferencia");

            return new FormaPagoDto
            {
                Id = formaPago.Id,
                Descripcion = formaPago.Descripcion,
                RequiereBanco = requiereBanco,
                RequiereTarjeta = formaPago.Descripcion.ToLower().Contains("tarjeta"),
                PermiteCuotas = formaPago.Descripcion.ToLower().Contains("crédito"),
                MaximoCuotas = 12
            };
        }
    }
}