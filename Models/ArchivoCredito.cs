using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("ArchivoCredito")]
public partial class ArchivoCredito
{
    [Key]
    public int Id { get; set; }

    public int SolicitudCreditoId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? TipoArchivo { get; set; }

    [Column("URL")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Url { get; set; }

    public DateTime FechaCarga { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("SolicitudCreditoId")]
    [InverseProperty("ArchivoCreditos")]
    public virtual SolicitudCredito SolicitudCredito { get; set; } = null!;
}
