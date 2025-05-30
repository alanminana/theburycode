using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("ScoringLog")]
public partial class ScoringLog
{
    [Key]
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public DateTime FechaCambio { get; set; }

    public byte? ScoringAnterior { get; set; }

    public byte ScoringNuevo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Usuario { get; set; } = null!;

    [ForeignKey("ClienteId")]
    [InverseProperty("ScoringLogs")]
    public virtual Cliente Cliente { get; set; } = null!;
}
