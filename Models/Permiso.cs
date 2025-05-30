using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("Permiso")]
[Index("Codigo", Name = "UQ_Permiso_Codigo", IsUnique = true)]
public partial class Permiso
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Codigo { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? Descripcion { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [InverseProperty("Permiso")]
    public virtual ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
}
