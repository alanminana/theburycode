
// Services/Implementations/VentaService.cs
using Microsoft.EntityFrameworkCore;
using theburycode.Models;

namespace theburycode.Services.Shared
{
    public interface IPrecioCalculatorService
    {
        decimal CalcularPrecioLista(decimal precioCosto, decimal margenPct);
        decimal CalcularPrecioContado(decimal precioLista, decimal descuentoPct);
        decimal CalcularPrecioFinal(decimal precioBase, decimal ivaPct);
        decimal CalcularDescuento(decimal precioOriginal, decimal precioConDescuento);
        Task<PrecioProductoDto?> GetPreciosProducto(int productoId);
        Task<decimal> AplicarAjusteMasivo(List<int> productosIds, decimal porcentajeAjuste, string tipoAjuste, string usuario);
    }    }