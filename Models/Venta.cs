using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Index("ClienteId", Name = "IX_Venta_ClienteId")]
[Index("Fecha", Name = "IX_Venta_Fecha")]
[Index("NumeroFactura", Name = "UQ_Venta_NumeroFactura", IsUnique = true)]
public partial class Venta
{
    [Key]
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public int FormaPagoId { get; set; }

    public int? BancoId { get; set; }

    public int? TipoTarjetaId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? NumeroFactura { get; set; }

    public DateTime Fecha { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Condiciones { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? EstadoEntrega { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ImporteTotal { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioModificacion { get; set; }

    [ForeignKey("BancoId")]
    [InverseProperty("Venta")]
    public virtual Banco? Banco { get; set; }

    [ForeignKey("ClienteId")]
    [InverseProperty("Venta")]
    public virtual Cliente Cliente { get; set; } = null!;

    [ForeignKey("EstadoEntrega")]
    [InverseProperty("Venta")]
    public virtual EstadoEntrega? EstadoEntregaNavigation { get; set; }

    [ForeignKey("FormaPagoId")]
    [InverseProperty("Venta")]
    public virtual FormaPago FormaPago { get; set; } = null!;

    [InverseProperty("Venta")]
    public virtual ICollection<Rmacliente> Rmaclientes { get; set; } = new List<Rmacliente>();

    [ForeignKey("TipoTarjetaId")]
    [InverseProperty("Venta")]
    public virtual TipoTarjetum? TipoTarjeta { get; set; }

    [InverseProperty("Venta")]
    public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
}
