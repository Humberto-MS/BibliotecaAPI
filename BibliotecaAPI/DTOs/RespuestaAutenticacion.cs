namespace BibliotecaAPI.DTOs {
    public class RespuestaAutenticacion {
        public required string Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
