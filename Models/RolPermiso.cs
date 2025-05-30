using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[PrimaryKey("RolId", "PermisoId")]
[Table("RolPermiso")]
public partial class RolPermiso
{
    [Key]
    public int RolId { get; set; }

    [Key]
    public int PermisoId { get; set; }

    public DateTime FechaAsignacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("PermisoId")]
    [InverseProperty("RolPermisos")]
    public virtual Permiso Permiso { get; set; } = null!;

    [ForeignKey("RolId")]
    [InverseProperty("RolPermisos")]
    public virtual Rol Rol { get; set; } = null!;
}
