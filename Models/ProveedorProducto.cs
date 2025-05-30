using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[PrimaryKey("ProveedorId", "ProductoId")]
[Table("ProveedorProducto")]
public partial class ProveedorProducto
{
    [Key]
    public int ProveedorId { get; set; }

    [Key]
    public int ProductoId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PrecioCosto { get; set; }

    public int? PlazoEntrega { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("ProductoId")]
    [InverseProperty("ProveedorProductos")]
    public virtual Producto Producto { get; set; } = null!;

    [ForeignKey("ProveedorId")]
    [InverseProperty("ProveedorProductos")]
    public virtual Proveedor Proveedor { get; set; } = null!;
}
