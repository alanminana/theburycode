using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("RMACliente")]
public partial class Rmacliente
{
    [Key]
    public int Id { get; set; }

    public int VentaId { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Tipo { get; set; }

    public DateTime FechaSolicitud { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Estado { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("Estado")]
    [InverseProperty("Rmaclientes")]
    public virtual EstadoRma? EstadoNavigation { get; set; }

    [InverseProperty("Rmacliente")]
    public virtual ICollection<RmaclienteDetalle> RmaclienteDetalles { get; set; } = new List<RmaclienteDetalle>();

    [ForeignKey("Tipo")]
    [InverseProperty("Rmaclientes")]
    public virtual TipoRmacliente? TipoNavigation { get; set; }

    [ForeignKey("VentaId")]
    [InverseProperty("Rmaclientes")]
    public virtual Venta Venta { get; set; } = null!;
}
