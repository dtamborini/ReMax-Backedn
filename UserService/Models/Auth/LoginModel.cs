using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Lo username è obbligatorio.")]
        public string Username { get; set; } = "testuser";

        [Required(ErrorMessage = "La password è obbligatoria.")]
        public string Password { get; set; } = "testpassword";

        [Required(ErrorMessage = "Il Client ID è obbligatorio.")]
        public string ClientId { get; set; } = "my-client";
        public string? ClientSecret { get; set; } = "my-client-secret";
    }
}