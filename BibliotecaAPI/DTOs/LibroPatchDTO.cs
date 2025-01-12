using BibliotecaAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs {
    public class LibroPatchDTO {
        [Required]
        [PrimeraLetraMayuscula]
        [StringLength ( maximumLength: 250 )]
        public required string Titulo { get; set; }

        public DateTime FechaPublicacion { get; set; }
    }
}
