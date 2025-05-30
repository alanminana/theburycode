using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

public partial class Categoria
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string Tipo { get; set; } = null!;

    public int? ParentId { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioModificacion { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<Categoria> InverseParent { get; set; } = new List<Categoria>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Categoria? Parent { get; set; }

    [InverseProperty("Categoria")]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
