using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Keyless]
public partial class VwProductosPreciosCalculado
{
    public int Id { get; set; }

    public int CodigoNum { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CodigoAlfaNum { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Categoria { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Marca { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? Submarca { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PrecioCosto { get; set; }

    [Column(TypeName = "decimal(29, 8)")]
    public decimal? PrecioLista { get; set; }

    [Column(TypeName = "decimal(38, 12)")]
    public decimal? PrecioContado { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? IvaPct { get; set; }

    public int? StockActual { get; set; }

    public int? StockMinimo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Estado { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public DateTime? FechaModificacion { get; set; }
}
