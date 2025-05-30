using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("Genero")]
public partial class Genero
{
    [Key]
    [StringLength(1)]
    [Unicode(false)]
    public string Codigo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("GeneroNavigation")]
    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    [InverseProperty("GeneroNavigation")]
    public virtual ICollection<Conyuge> Conyuges { get; set; } = new List<Conyuge>();

    [InverseProperty("GeneroNavigation")]
    public virtual ICollection<Garante> Garantes { get; set; } = new List<Garante>();
}
