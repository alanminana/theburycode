using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("Garante")]
public partial class Garante
{
    [Key]
    public int Id { get; set; }

    public int ClienteId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Apellido { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string? Genero { get; set; }

    [Column("DNI")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Dni { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Telefono { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Parentesco { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("ClienteId")]
    [InverseProperty("Garantes")]
    public virtual Cliente Cliente { get; set; } = null!;

    [ForeignKey("Genero")]
    [InverseProperty("Garantes")]
    public virtual Genero? GeneroNavigation { get; set; }
}
