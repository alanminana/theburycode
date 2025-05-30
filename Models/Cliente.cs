using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace theburycode.Models;

[Table("Cliente")]
[Index("Apellido", "Nombre", Name = "IX_Cliente_ApellidoNombre")]
[Index("Dni", Name = "IX_Cliente_DNI")]
[Index("Dni", Name = "UQ_Cliente_DNI", IsUnique = true)]
public partial class Cliente
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Apellido { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string Genero { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string? EstadoCivil { get; set; }

    [Column("DNI")]
    [StringLength(20)]
    [Unicode(false)]
    public string Dni { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? Telefono { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Celular { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public DateTime FechaAlta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UsuarioAlta { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UsuarioModificacion { get; set; }

    public int? Scoring { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ContactoEmergencia { get; set; }

    [Unicode(false)]
    public string? Notas { get; set; }

    public int CiudadId { get; set; }

    [ForeignKey("CiudadId")]
    [InverseProperty("Clientes")]
    public virtual Ciudad Ciudad { get; set; } = null!;

    [InverseProperty("Cliente")]
    public virtual ICollection<Conyuge> Conyuges { get; set; } = new List<Conyuge>();

    [InverseProperty("Cliente")]
    public virtual ICollection<Cotizacion> Cotizacions { get; set; } = new List<Cotizacion>();

    [InverseProperty("Cliente")]
    public virtual ICollection<DocumentoCliente> DocumentoClientes { get; set; } = new List<DocumentoCliente>();

    [InverseProperty("Cliente")]
    public virtual DomicilioLaboral? DomicilioLaboral { get; set; }

    [InverseProperty("Cliente")]
    public virtual DomicilioParticular? DomicilioParticular { get; set; }

    [ForeignKey("EstadoCivil")]
    [InverseProperty("Clientes")]
    public virtual EstadoCivil? EstadoCivilNavigation { get; set; }

    [InverseProperty("Cliente")]
    public virtual ICollection<Garante> Garantes { get; set; } = new List<Garante>();

    [ForeignKey("Genero")]
    [InverseProperty("Clientes")]
    public virtual Genero GeneroNavigation { get; set; } = null!;

    [InverseProperty("Cliente")]
    public virtual ICollection<ScoringLog> ScoringLogs { get; set; } = new List<ScoringLog>();

    [InverseProperty("Cliente")]
    public virtual ICollection<SolicitudCredito> SolicitudCreditos { get; set; } = new List<SolicitudCredito>();

    [InverseProperty("Cliente")]
    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
