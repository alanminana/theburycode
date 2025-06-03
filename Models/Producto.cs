using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("Producto")]
[Index("CodigoNum", Name = "IX_Producto_CodigoNum")]
[Index("Nombre", Name = "IX_Producto_Nombre")]
[Index("CodigoAlfaNum", Name = "UQ_Producto_CodigoAlfaNum", IsUnique = true)]
[Index("CodigoNum", Name = "UQ_Producto_CodigoNum", IsUnique = true)]
public partial class Producto
{
    [Key]
    public int Id { get; set; }
    public string? ImagenUrl { get; set; }

    public int CodigoNum { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CodigoAlfaNum { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    public int CategoriaId { get; set; }

    public int MarcaId { get; set; }

    public int? SubmarcaId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PrecioCosto { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? MargenVentaPct { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? DescuentoContadoPct { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? IvaPct { get; set; }

    public int? StockActual { get; set; }

    public int? StockMinimo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? OrigenStock { get; set; }

    [Unicode(false)]
    public string Descripcion { get; set; } = null!;

    public bool Activo { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string EstadoProducto { get; set; } = null!;

    public DateTime? FechaSuspension { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioModificacion { get; set; }

    [ForeignKey("CategoriaId")]
    [InverseProperty("Productos")]
    public virtual Categoria Categoria { get; set; } = null!;

    [InverseProperty("Producto")]
    public virtual ICollection<CompraDetalle> CompraDetalles { get; set; } = new List<CompraDetalle>();

    [InverseProperty("Producto")]
    public virtual ICollection<CotizacionDetalle> CotizacionDetalles { get; set; } = new List<CotizacionDetalle>();

    [ForeignKey("EstadoProducto")]
    [InverseProperty("Productos")]
    public virtual EstadoProducto EstadoProductoNavigation { get; set; } = null!;

    [ForeignKey("MarcaId")]
    [InverseProperty("ProductoMarcas")]
    public virtual Marca Marca { get; set; } = null!;

    [InverseProperty("Producto")]
    public virtual ICollection<PrecioLog> PrecioLogs { get; set; } = new List<PrecioLog>();

    [InverseProperty("Producto")]
    public virtual ICollection<ProveedorProducto> ProveedorProductos { get; set; } = new List<ProveedorProducto>();

    [InverseProperty("Producto")]
    public virtual ICollection<RmaclienteDetalle> RmaclienteDetalles { get; set; } = new List<RmaclienteDetalle>();

    [InverseProperty("Producto")]
    public virtual ICollection<Rmaproveedor> Rmaproveedors { get; set; } = new List<Rmaproveedor>();

    [InverseProperty("Producto")]
    public virtual ICollection<StockLog> StockLogs { get; set; } = new List<StockLog>();

    [ForeignKey("SubmarcaId")]
    [InverseProperty("ProductoSubmarcas")]
    public virtual Marca? Submarca { get; set; }

    [InverseProperty("Producto")]
    public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
}
