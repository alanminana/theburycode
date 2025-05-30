using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("StockLog")]
public partial class StockLog
{
    [Key]
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public int? CantidadAnterior { get; set; }

    public int? CantidadNueva { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? TipoMovimiento { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Origen { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Usuario { get; set; }

    [ForeignKey("ProductoId")]
    [InverseProperty("StockLogs")]
    public virtual Producto Producto { get; set; } = null!;
}
