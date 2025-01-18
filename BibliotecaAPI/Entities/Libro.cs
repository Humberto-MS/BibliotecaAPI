using BibliotecaAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entities
{
    public class Libro
    {
        public int Id { get; set; }

        [Required]
        [PrimeraLetraMayuscula]
        [StringLength ( maximumLength: 250 )]
        public required string Titulo { get; set; }

        public List<Comentario> Comentarios { get; set; } = [];
        public List<AutorLibro> AutoresLibros { get; set; } = [];
        public DateTime? FechaPublicacion { get; set; }
    }
}
