// Services/Shared/IComprobanteService.cs
using Microsoft.EntityFrameworkCore;
using theburycode.Models;

namespace theburycode.Services.Shared
{

    public class ComprobanteService : IComprobanteService
    {
        private readonly TheBuryCodeContext _context;

        public ComprobanteService(TheBuryCodeContext context)
        {
            _context = context;
        }

        public async Task<string> GenerarNumero(string prefijo)
        {
            var fecha = DateTime.Now;
            var formato = $"{prefijo}-{fecha:yyyyMMdd}-";

            // Buscar el último número según el prefijo
            string? ultimoNumero = null;
            switch (prefijo)
            {
                case "FAC":
                    ultimoNumero = await _context.Venta
                        .Where(v => v.NumeroFactura != null && v.NumeroFactura.StartsWith(formato))
                        .OrderByDescending(v => v.NumeroFactura)
                        .Select(v => v.NumeroFactura)
                        .FirstOrDefaultAsync();
                    break;
                case "COT":
                    ultimoNumero = await _context.Cotizacions
                        .Where(c => c.NumeroCotizacion != null && c.NumeroCotizacion.StartsWith(formato))
                        .OrderByDescending(c => c.NumeroCotizacion)
                        .Select(c => c.NumeroCotizacion)
                        .FirstOrDefaultAsync();
                    break;
                case "OC":
                    ultimoNumero = await _context.Compras
                        .Where(c => c.NumeroFactura != null && c.NumeroFactura.StartsWith(formato))
                        .OrderByDescending(c => c.NumeroFactura)
                        .Select(c => c.NumeroFactura)
                        .FirstOrDefaultAsync();
                    break;
            }

            if (string.IsNullOrEmpty(ultimoNumero))
            {
                return $"{formato}0001";
            }

            var numeroActual = int.Parse(ultimoNumero.Substring(formato.Length));
            return $"{formato}{(numeroActual + 1):D4}";
        }

        public async Task<string> GenerarNumeroFactura() => await GenerarNumero("FAC");
        public async Task<string> GenerarNumeroCotizacion() => await GenerarNumero("COT");
        public async Task<string> GenerarNumeroCompra() => await GenerarNumero("OC");
        public async Task<string> GenerarNumeroRMA() => await GenerarNumero("RMA");

        public bool ValidarFormato(string numero, string prefijo)
        {
            if (string.IsNullOrEmpty(numero)) return false;

            var pattern = $@"^{prefijo}-\d{{8}}-\d{{4}}$";
            return System.Text.RegularExpressions.Regex.IsMatch(numero, pattern);
        }
    }
}
