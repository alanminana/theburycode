using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("PrecioLog")]
public partial class PrecioLog
{
    [Key]
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public DateTime FechaCambio { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PrecioAnterior { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PrecioNuevo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Usuario { get; set; }

    [ForeignKey("ProductoId")]
    [InverseProperty("PrecioLogs")]
    public virtual Producto Producto { get; set; } = null!;
}
