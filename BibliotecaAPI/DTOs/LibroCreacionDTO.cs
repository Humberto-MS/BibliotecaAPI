using BibliotecaAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs {
    public class LibroCreacionDTO {
        [Required]
        [PrimeraLetraMayuscula]
        [StringLength ( maximumLength: 250 )]
        public required string Titulo { get; set; }

        public required List<int> AutoresIds { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
