using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[PrimaryKey("VentaId", "ProductoId")]
[Table("VentaDetalle")]
public partial class VentaDetalle
{
    [Key]
    public int VentaId { get; set; }

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

    [ForeignKey("ProductoId")]
    [InverseProperty("VentaDetalles")]
    public virtual Producto Producto { get; set; } = null!;

    [ForeignKey("VentaId")]
    [InverseProperty("VentaDetalles")]
    public virtual Venta Venta { get; set; } = null!;
}
