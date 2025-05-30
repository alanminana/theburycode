using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Index("Estado", Name = "IX_Cuota_Estado")]
[Index("FechaVencimiento", Name = "IX_Cuota_FechaVencimiento")]
[Index("LineaCreditoId", "NumeroCuota", Name = "UQ_Cuota_LineaCredito_Numero", IsUnique = true)]
public partial class Cuota
{
    [Key]
    public int Id { get; set; }

    public int LineaCreditoId { get; set; }

    public int NumeroCuota { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MontoCapital { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MontoInteres { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MontoMora { get; set; }

    [Column(TypeName = "decimal(20, 2)")]
    public decimal? MontoTotal { get; set; }

    public DateTime? FechaPago { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? MontoPagado { get; set; }

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

    [ForeignKey("Estado")]
    [InverseProperty("Cuota")]
    public virtual EstadoCuota EstadoNavigation { get; set; } = null!;

    [ForeignKey("LineaCreditoId")]
    [InverseProperty("Cuota")]
    public virtual LineaCredito LineaCredito { get; set; } = null!;
}
