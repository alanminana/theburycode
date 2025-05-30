using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("Configuracion")]
public partial class Configuracion
{
    [Key]
    [StringLength(100)]
    [Unicode(false)]
    public string Key { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? Valor { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Descripcion { get; set; }

    public int? ModuloId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Categoria { get; set; }

    public DateTime? FechaModificacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioModificacion { get; set; }

    [ForeignKey("ModuloId")]
    [InverseProperty("Configuracions")]
    public virtual SysModulo? Modulo { get; set; }
}
