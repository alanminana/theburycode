using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("Cotizacion")]
[Index("NumeroCotizacion", Name = "UQ_Cotizacion_NumeroCotizacion", IsUnique = true)]
public partial class Cotizacion
{
    [Key]
    public int Id { get; set; }

    public int ClienteId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? NumeroCotizacion { get; set; }

    public DateTime Fecha { get; set; }

    public int? VigenciaDays { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioModificacion { get; set; }

    [ForeignKey("ClienteId")]
    [InverseProperty("Cotizacions")]
    public virtual Cliente Cliente { get; set; } = null!;

    [InverseProperty("Cotizacion")]
    public virtual ICollection<CotizacionDetalle> CotizacionDetalles { get; set; } = new List<CotizacionDetalle>();
}
