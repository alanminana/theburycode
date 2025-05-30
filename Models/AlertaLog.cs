using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("AlertaLog")]
public partial class AlertaLog
{
    [Key]
    public int Id { get; set; }

    public int AlertaId { get; set; }

    public DateTime FechaGeneracion { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Estado { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Usuario { get; set; }

    [ForeignKey("AlertaId")]
    [InverseProperty("AlertaLogs")]
    public virtual Alertum Alerta { get; set; } = null!;
}
