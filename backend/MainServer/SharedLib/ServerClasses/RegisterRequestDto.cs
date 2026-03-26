
using SharedLib.Models;
using System.ComponentModel.DataAnnotations;

namespace SharedLib.ServerClasses
{
    public class RegisterRequestDto
    {
        [Required, MinLength(3), MaxLength(50)] public string UserName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)] public string Password { get; set; } = string.Empty;
    }
}
