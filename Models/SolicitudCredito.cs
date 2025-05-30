using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("SolicitudCredito")]
[Index("ClienteId", Name = "IX_SolicitudCredito_ClienteId")]
public partial class SolicitudCredito
{
    [Key]
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public DateTime FechaSolicitud { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? MontoSolicitado { get; set; }

    public int? PlazoCuotas { get; set; }

    public bool? TieneReciboSueldo { get; set; }

    public bool? TieneGarante { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? ServicioNombreTitular { get; set; }

    [Column("VerazOK")]
    public bool? VerazOk { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Estado { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioSolicitante { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioEvaluador { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioModificacion { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal InteresBasePct { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal InteresMoraPct { get; set; }

    [InverseProperty("SolicitudCredito")]
    public virtual ICollection<ArchivoCredito> ArchivoCreditos { get; set; } = new List<ArchivoCredito>();

    [ForeignKey("ClienteId")]
    [InverseProperty("SolicitudCreditos")]
    public virtual Cliente Cliente { get; set; } = null!;

    [ForeignKey("Estado")]
    [InverseProperty("SolicitudCreditos")]
    public virtual EstadoSolicitudCredito? EstadoNavigation { get; set; }

    [InverseProperty("SolicitudCredito")]
    public virtual ICollection<LineaCredito> LineaCreditos { get; set; } = new List<LineaCredito>();
}
