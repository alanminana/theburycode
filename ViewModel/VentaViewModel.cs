
// ViewModels/VentaViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace theburycode.ViewModels
{
    public class VentaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El cliente es requerido")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Display(Name = "Cliente")]
        public string? ClienteNombre { get; set; }

        [Display(Name = "DNI Cliente")]
        public string? ClienteDni { get; set; }

        [Required(ErrorMessage = "La forma de pago es requerida")]
        [Display(Name = "Forma de Pago")]
        public int FormaPagoId { get; set; }

        [Display(Name = "Banco")]
        public int? BancoId { get; set; }

        [Display(Name = "Tipo Tarjeta")]
        public int? TipoTarjetaId { get; set; }

        [Display(Name = "Número Factura")]
        public string? NumeroFactura { get; set; }

        [Display(Name = "Fecha")]
        [DataType(DataType.DateTime)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "Condiciones")]
        [StringLength(200)]
        public string? Condiciones { get; set; }

        [Display(Name = "Estado Entrega")]
        public string? EstadoEntrega { get; set; }

        [Display(Name = "Importe Total")]
        [DataType(DataType.Currency)]
        public decimal? ImporteTotal { get; set; }

        // Detalles de la venta
        public List<VentaDetalleViewModel> Detalles { get; set; } = new();

        // Para búsqueda de cliente
        [Display(Name = "Buscar por DNI")]
        public string? BuscarDni { get; set; }
    }

    public class VentaDetalleViewModel
    {
        public int ProductoId { get; set; }
        public string? ProductoNombre { get; set; }
        public string? ProductoCodigo { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 9999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnit { get; set; }

        public decimal Subtotal => Cantidad * PrecioUnit;
    }
}