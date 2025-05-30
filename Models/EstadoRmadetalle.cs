using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("EstadoRMADetalle")]
public partial class EstadoRmadetalle
{
    [Key]
    [StringLength(1)]
    [Unicode(false)]
    public string Codigo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("EstadoNavigation")]
    public virtual ICollection<RmaclienteDetalle> RmaclienteDetalles { get; set; } = new List<RmaclienteDetalle>();
}
