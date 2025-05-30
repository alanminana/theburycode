using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("EstadoProducto")]
public partial class EstadoProducto
{
    [Key]
    [StringLength(1)]
    [Unicode(false)]
    public string Codigo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("EstadoProductoNavigation")]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
