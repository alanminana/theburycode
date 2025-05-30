using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("DomicilioLaboral")]
public partial class DomicilioLaboral
{
    internal readonly string CalleYNumero;

    [Key]
    public int ClienteId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? NombreEmpresa { get; set; }

    [Column("CalleYNumero")]
    [StringLength(200)]
    [Unicode(false)]
    public string? CalleYnumero { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? DescripcionDomicilioLaboral { get; set; }

    public int CiudadLaboralId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? TelefonoLaboral { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("CiudadLaboralId")]
    [InverseProperty("DomicilioLaborals")]
    public virtual Ciudad CiudadLaboral { get; set; } = null!;

    [ForeignKey("ClienteId")]
    [InverseProperty("DomicilioLaboral")]
    public virtual Cliente Cliente { get; set; } = null!;
}
