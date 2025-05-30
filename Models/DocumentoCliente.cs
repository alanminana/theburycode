using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("DocumentoCliente")]
public partial class DocumentoCliente
{
    [Key]
    public int Id { get; set; }

    public int ClienteId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string TipoDocumento { get; set; } = null!;

    [Column("URL")]
    [StringLength(200)]
    [Unicode(false)]
    public string Url { get; set; } = null!;

    public DateTime FechaCarga { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    [ForeignKey("ClienteId")]
    [InverseProperty("DocumentoClientes")]
    public virtual Cliente Cliente { get; set; } = null!;
}
