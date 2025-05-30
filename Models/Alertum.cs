using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

public partial class Alertum
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Modulo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? TipoAlerta { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Parametro { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [InverseProperty("Alerta")]
    public virtual ICollection<AlertaLog> AlertaLogs { get; set; } = new List<AlertaLog>();
}
