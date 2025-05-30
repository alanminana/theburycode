using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("SysModulo")]
[Index("Nombre", Name = "UQ_SysModulo_Nombre", IsUnique = true)]
public partial class SysModulo
{
    [Key]
    public int ModuloId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("Modulo")]
    public virtual ICollection<Configuracion> Configuracions { get; set; } = new List<Configuracion>();
}
