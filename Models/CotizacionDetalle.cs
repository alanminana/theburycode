using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[PrimaryKey("CotizacionId", "ProductoId")]
[Table("CotizacionDetalle")]
public partial class CotizacionDetalle
{
    [Key]
    public int CotizacionId { get; set; }

    [Key]
    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PrecioUnit { get; set; }

    [Column(TypeName = "decimal(29, 2)")]
    public decimal? Subtotal { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("CotizacionId")]
    [InverseProperty("CotizacionDetalles")]
    public virtual Cotizacion Cotizacion { get; set; } = null!;

    [ForeignKey("ProductoId")]
    [InverseProperty("CotizacionDetalles")]
    public virtual Producto Producto { get; set; } = null!;
}
