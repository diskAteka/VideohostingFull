using System.Security.Claims;

namespace SharedLib.GlobalInterfaces
{
    public interface ITokenService
    {
        public string GenerateToken(int userId, string userName, string email,
            bool canUpload);
        ClaimsPrincipal ValidateToken(string token);
    }
}
