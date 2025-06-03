// ViewModels/ClienteViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace theburycode.ViewModels
{
    public class ClienteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder 50 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es requerido")]
        [StringLength(20, ErrorMessage = "El DNI no puede exceder 20 caracteres")]
        [Display(Name = "DNI")]
        public string Dni { get; set; } = string.Empty;

        [Required(ErrorMessage = "El género es requerido")]
        [Display(Name = "Género")]
        public string Genero { get; set; } = string.Empty;

        [Display(Name = "Estado Civil")]
        public string? EstadoCivil { get; set; }

        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        public string? Telefono { get; set; }

        [Display(Name = "Celular")]
        [Phone(ErrorMessage = "Formato de celular inválido")]
        public string? Celular { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string? Email { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "La ciudad es requerida")]
        public int CiudadId { get; set; }

        [Range(1, 10, ErrorMessage = "El scoring debe estar entre 1 y 10")]
        public int? Scoring { get; set; }

        [Display(Name = "Contacto de Emergencia")]
        public string? ContactoEmergencia { get; set; }

        [Display(Name = "Notas")]
        [DataType(DataType.MultilineText)]
        public string? Notas { get; set; }

        // Propiedades de solo lectura para mostrar
        public string NombreCompleto => $"{Apellido}, {Nombre}";
        public string? CiudadNombre { get; set; }
        public string? ProvinciaNombre { get; set; }

        // Para el domicilio particular
        public DomicilioViewModel? DomicilioParticular { get; set; }

        // Para el domicilio laboral
        public DomicilioLaboralViewModel? DomicilioLaboral { get; set; }
    }

    public class DomicilioViewModel
    {
        [Display(Name = "Calle y Número")]
        [StringLength(200)]
        public string? CalleYNumero { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(200)]
        public string? DescripcionDomicilio { get; set; }
    }

    public class DomicilioLaboralViewModel
    {
        [Display(Name = "Empresa")]
        [StringLength(100)]
        public string? NombreEmpresa { get; set; }

        [Display(Name = "Calle y Número")]
        [StringLength(200)]
        public string? CalleYNumero { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(200)]
        public string? DescripcionDomicilioLaboral { get; set; }

        [Display(Name = "Ciudad Laboral")]
        public int CiudadLaboralId { get; set; }

        [Display(Name = "Teléfono Laboral")]
        [Phone]
        public string? TelefonoLaboral { get; set; }
    }
}