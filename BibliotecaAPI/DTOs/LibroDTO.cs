using BibliotecaAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs {
    public class LibroDTO {
        public int Id { get; set; }
        public required string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        //public required List<ComentarioDTO> Comentarios { get; set; }
    }
}
