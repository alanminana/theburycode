using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[PrimaryKey("UsuarioId", "RolId")]
[Table("UsuarioRol")]
public partial class UsuarioRol
{
    [Key]
    public int UsuarioId { get; set; }

    [Key]
    public int RolId { get; set; }

    public DateTime FechaAsignacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("RolId")]
    [InverseProperty("UsuarioRols")]
    public virtual Rol Rol { get; set; } = null!;

    [ForeignKey("UsuarioId")]
    [InverseProperty("UsuarioRols")]
    public virtual Usuario Usuario { get; set; } = null!;
}
