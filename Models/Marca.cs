using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("Marca")]
public partial class Marca
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    public int? ParentId { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioModificacion { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Tipo { get; set; } = null!;

    [InverseProperty("Parent")]
    public virtual ICollection<Marca> InverseParent { get; set; } = new List<Marca>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Marca? Parent { get; set; }

    [InverseProperty("Marca")]
    public virtual ICollection<Producto> ProductoMarcas { get; set; } = new List<Producto>();

    [InverseProperty("Submarca")]
    public virtual ICollection<Producto> ProductoSubmarcas { get; set; } = new List<Producto>();
}
