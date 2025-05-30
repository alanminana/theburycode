using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("Compra")]
[Index("Fecha", Name = "IX_Compra_Fecha")]
[Index("NumeroFactura", Name = "UQ_Compra_NumeroFactura", IsUnique = true)]
public partial class Compra
{
    [Key]
    public int Id { get; set; }

    public int ProveedorId { get; set; }

    public int FormaPagoId { get; set; }

    public int? BancoId { get; set; }

    public int? TipoTarjetaId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? NumeroFactura { get; set; }

    public DateTime Fecha { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ImporteTotal { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Estado { get; set; }

    public int? Cuotas { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? EntidadElectronica { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioModificacion { get; set; }

    [ForeignKey("BancoId")]
    [InverseProperty("Compras")]
    public virtual Banco? Banco { get; set; }

    [InverseProperty("Compra")]
    public virtual ICollection<CompraDetalle> CompraDetalles { get; set; } = new List<CompraDetalle>();

    [ForeignKey("Estado")]
    [InverseProperty("Compras")]
    public virtual EstadoCompra? EstadoNavigation { get; set; }

    [ForeignKey("FormaPagoId")]
    [InverseProperty("Compras")]
    public virtual FormaPago FormaPago { get; set; } = null!;

    [ForeignKey("ProveedorId")]
    [InverseProperty("Compras")]
    public virtual Proveedor Proveedor { get; set; } = null!;

    [ForeignKey("TipoTarjetaId")]
    [InverseProperty("Compras")]
    public virtual TipoTarjetum? TipoTarjeta { get; set; }
}
