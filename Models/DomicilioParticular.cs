using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("DomicilioParticular")]
public partial class DomicilioParticular
{
    [Key]
    public int ClienteId { get; set; }

    [Column("CalleYNumero")]
    [StringLength(200)]
    [Unicode(false)]
    public string? CalleYnumero { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? DescripcionDomicilio { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("ClienteId")]
    [InverseProperty("DomicilioParticular")]
    public virtual Cliente Cliente { get; set; } = null!;
}
