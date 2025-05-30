using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("Ciudad")]
public partial class Ciudad
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    public int ProvinciaId { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioModificacion { get; set; }

    [InverseProperty("Ciudad")]
    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    [InverseProperty("CiudadLaboral")]
    public virtual ICollection<DomicilioLaboral> DomicilioLaborals { get; set; } = new List<DomicilioLaboral>();

    [ForeignKey("ProvinciaId")]
    [InverseProperty("Ciudads")]
    public virtual Provincium Provincia { get; set; } = null!;
}
