using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs {
    public class EditarAdminDTO {

        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
