// Services/Shared/IPrecioCalculatorService.cs
using theburycode.Models;

namespace theburycode.Services.Shared
{
  
    public class PrecioProductoDto
    {
        public int ProductoId { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal PrecioLista { get; set; }
        public decimal PrecioContado { get; set; }
        public decimal PrecioFinal { get; set; }
        public decimal MargenGanancia { get; set; }
        public decimal MargenPorcentaje { get; set; }
    }

    public class PrecioCalculatorService : IPrecioCalculatorService
    {
        private readonly TheBuryCodeContext _context;

        public PrecioCalculatorService(TheBuryCodeContext context)
        {
            _context = context;
        }

        public decimal CalcularPrecioLista(decimal precioCosto, decimal margenPct)
        {
            return precioCosto * (1 + margenPct / 100);
        }

        public decimal CalcularPrecioContado(decimal precioLista, decimal descuentoPct)
        {
            return precioLista * (1 - descuentoPct / 100);
        }

        public decimal CalcularPrecioFinal(decimal precioBase, decimal ivaPct)
        {
            return precioBase * (1 + ivaPct / 100);
        }

        public decimal CalcularDescuento(decimal precioOriginal, decimal precioConDescuento)
        {
            if (precioOriginal == 0) return 0;
            return ((precioOriginal - precioConDescuento) / precioOriginal) * 100;
        }

        public async Task<PrecioProductoDto?> GetPreciosProducto(int productoId)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return null;

            var precioCosto = producto.PrecioCosto ?? 0;
            var precioLista = CalcularPrecioLista(precioCosto, producto.MargenVentaPct ?? 0);
            var precioContado = CalcularPrecioContado(precioLista, producto.DescuentoContadoPct ?? 0);
            var precioFinal = CalcularPrecioFinal(precioContado, producto.IvaPct ?? 21);

            return new PrecioProductoDto
            {
                ProductoId = productoId,
                PrecioCosto = precioCosto,
                PrecioLista = precioLista,
                PrecioContado = precioContado,
                PrecioFinal = precioFinal,
                MargenGanancia = precioLista - precioCosto,
                MargenPorcentaje = precioCosto > 0 ? ((precioLista - precioCosto) / precioCosto) * 100 : 0
            };
        }

        public async Task<decimal> AplicarAjusteMasivo(List<int> productosIds, decimal porcentajeAjuste, string tipoAjuste, string usuario)
        {
            var productosActualizados = 0m;

            foreach (var id in productosIds)
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null) continue;

                var precioAnterior = producto.PrecioCosto ?? 0;
                decimal precioNuevo = tipoAjuste == "AUMENTO"
                    ? precioAnterior * (1 + porcentajeAjuste / 100)
                    : precioAnterior * (1 - porcentajeAjuste / 100);

                producto.PrecioCosto = precioNuevo;
                producto.FechaModificacion = DateTime.Now;
                producto.UsuarioModificacion = usuario;

                // Log de precio
                _context.PrecioLogs.Add(new PrecioLog
                {
                    ProductoId = id,
                    FechaCambio = DateTime.Now,
                    PrecioAnterior = precioAnterior,
                    PrecioNuevo = precioNuevo,
                    Usuario = usuario
                });

                productosActualizados++;
            }

            await _context.SaveChangesAsync();
            return productosActualizados;
        }
    }
}