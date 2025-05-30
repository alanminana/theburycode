using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("EstadoEntrega")]
public partial class EstadoEntrega
{
    [Key]
    [StringLength(1)]
    [Unicode(false)]
    public string Codigo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("EstadoEntregaNavigation")]
    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
