using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("LineaCredito")]
public partial class LineaCredito
{
    [Key]
    public int Id { get; set; }

    public int SolicitudCreditoId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MontoAprobado { get; set; }

    public DateTime FechaAprobacion { get; set; }

    public DateTime FechaVencimiento { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SaldoDisponible { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Estado { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioModificacion { get; set; }

    [InverseProperty("LineaCredito")]
    public virtual ICollection<Cuota> Cuota { get; set; } = new List<Cuota>();

    [ForeignKey("SolicitudCreditoId")]
    [InverseProperty("LineaCreditos")]
    public virtual SolicitudCredito SolicitudCredito { get; set; } = null!;
}
