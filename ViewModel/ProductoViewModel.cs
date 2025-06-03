
// ViewModels/ProductoViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace theburycode.ViewModels
{
    public class ProductoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El c�digo num�rico es requerido")]
        [Display(Name = "C�digo")]
        public int CodigoNum { get; set; }

        [Required(ErrorMessage = "El c�digo alfanum�rico es requerido")]
        [StringLength(50)]
        [Display(Name = "C�digo Producto")]
        public string CodigoAlfaNum { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categor�a es requerida")]
        [Display(Name = "Categor�a (Subrubro)")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "La marca es requerida")]
        [Display(Name = "Marca")]
        public int MarcaId { get; set; }

        [Display(Name = "Submarca")]
        public int? SubmarcaId { get; set; }

        [Display(Name = "Precio Costo")]
        [DataType(DataType.Currency)]
        [Range(0, 9999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal? PrecioCosto { get; set; }

        [Display(Name = "Margen Venta %")]
        [Range(0, 999.99, ErrorMessage = "El margen debe estar entre 0 y 999.99")]
        public decimal? MargenVentaPct { get; set; }

        [Display(Name = "Descuento Contado %")]
        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100")]
        public decimal? DescuentoContadoPct { get; set; }

        [Display(Name = "IVA %")]
        [Range(0, 100, ErrorMessage = "El IVA debe estar entre 0 y 100")]
        public decimal? IvaPct { get; set; } = 21;

        [Display(Name = "Stock Actual")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int? StockActual { get; set; }

        [Display(Name = "Stock M�nimo")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock m�nimo no puede ser negativo")]
        public int? StockMinimo { get; set; }

        [Display(Name = "Origen Stock")]
        public string? OrigenStock { get; set; }

        [Display(Name = "Descripci�n")]
        [DataType(DataType.MultilineText)]
        public string? Descripcion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Estado")]
        public string EstadoProducto { get; set; } = "A";
     

        [Display(Name = "Imagen")]
        public IFormFile? ImagenFile { get; set; }

        public string? ImagenUrl { get; set; }
        // Propiedades calculadas
        public decimal PrecioLista => (PrecioCosto ?? 0) * (1 + (MargenVentaPct ?? 0) / 100);
        public decimal PrecioContado => PrecioLista * (1 - (DescuentoContadoPct ?? 0) / 100);
        public decimal PrecioFinal => PrecioContado * (1 + (IvaPct ?? 21) / 100);

        public string? CategoriaNombre { get; set; }
        public string? MarcaNombre { get; set; }
        public string? SubmarcaNombre { get; set; }
        public string NombreCompleto => $"{CodigoAlfaNum} - {Nombre}";
    }

    public class ProductoFiltroViewModel
    {
        public string? Busqueda { get; set; }
        public int? CategoriaId { get; set; }
        public int? MarcaId { get; set; }
        public bool? SoloActivos { get; set; } = true;
        public bool? SoloStockBajo { get; set; }
        public decimal? PrecioDesde { get; set; }
        public decimal? PrecioHasta { get; set; }
    }
}