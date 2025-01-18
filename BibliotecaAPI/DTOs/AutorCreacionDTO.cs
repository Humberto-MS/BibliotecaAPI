using BibliotecaAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs {
    public class AutorCreacionDTO {
        [Required ( ErrorMessage = "El campo {0} es requerido" )]
        [StringLength ( maximumLength: 120, ErrorMessage = "El campo {0} no debe de tener mas de {1} caracteres" )]
        [PrimeraLetraMayuscula]
        public required string Nombre { get; set; }

        public List<LibroCreacionDTO> Libros { get; set; } = [];
    }
}
