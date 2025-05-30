using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("RMAProveedor")]
public partial class Rmaproveedor
{
    [Key]
    public int Id { get; set; }

    public int ProveedorId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Motivo { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Estado { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("Estado")]
    [InverseProperty("Rmaproveedors")]
    public virtual EstadoRma? EstadoNavigation { get; set; }

    [ForeignKey("ProductoId")]
    [InverseProperty("Rmaproveedors")]
    public virtual Producto Producto { get; set; } = null!;

    [ForeignKey("ProveedorId")]
    [InverseProperty("Rmaproveedors")]
    public virtual Proveedor Proveedor { get; set; } = null!;
}
