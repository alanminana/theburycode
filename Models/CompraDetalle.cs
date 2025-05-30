using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[PrimaryKey("CompraId", "ProductoId")]
[Table("CompraDetalle")]
public partial class CompraDetalle
{
    [Key]
    public int CompraId { get; set; }

    [Key]
    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PrecioUnitario { get; set; }

    [Column(TypeName = "decimal(29, 2)")]
    public decimal? Subtotal { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("CompraId")]
    [InverseProperty("CompraDetalles")]
    public virtual Compra Compra { get; set; } = null!;

    [ForeignKey("ProductoId")]
    [InverseProperty("CompraDetalles")]
    public virtual Producto Producto { get; set; } = null!;
}
