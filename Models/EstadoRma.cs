using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("EstadoRMA")]
public partial class EstadoRma
{
    [Key]
    [StringLength(1)]
    [Unicode(false)]
    public string Codigo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("EstadoNavigation")]
    public virtual ICollection<Rmacliente> Rmaclientes { get; set; } = new List<Rmacliente>();

    [InverseProperty("EstadoNavigation")]
    public virtual ICollection<Rmaproveedor> Rmaproveedors { get; set; } = new List<Rmaproveedor>();
}
