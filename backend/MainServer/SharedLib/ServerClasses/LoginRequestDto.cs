using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SharedLib.ServerClasses
{
    public class LoginRequestDto
    {
        public string? Email { get; set; }
        [JsonPropertyName("username")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль минимум 6 символов")]
        public string Password { get; set; } = string.Empty;
    }
}
