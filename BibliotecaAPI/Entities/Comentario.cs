using Microsoft.AspNetCore.Identity;

namespace BibliotecaAPI.Entities {
    public class Comentario {
        public int Id { get; set; }
        public required string Contenido { get; set; }
        public int LibroId { get; set; }
        public required Libro Libro { get; set; }
        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }
    }
}
