using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[PrimaryKey("RmaclienteId", "ProductoId")]
[Table("RMAClienteDetalle")]
public partial class RmaclienteDetalle
{
    [Key]
    [Column("RMAClienteId")]
    public int RmaclienteId { get; set; }

    [Key]
    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Estado { get; set; }

    public bool? Danado { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("Estado")]
    [InverseProperty("RmaclienteDetalles")]
    public virtual EstadoRmadetalle? EstadoNavigation { get; set; }

    [ForeignKey("ProductoId")]
    [InverseProperty("RmaclienteDetalles")]
    public virtual Producto Producto { get; set; } = null!;

    [ForeignKey("RmaclienteId")]
    [InverseProperty("RmaclienteDetalles")]
    public virtual Rmacliente Rmacliente { get; set; } = null!;
}
