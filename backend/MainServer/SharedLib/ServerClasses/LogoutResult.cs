using SharedLib.Models;

namespace SharedLib.ServerClasses
{
    public class LogoutResultDto
    {
        public bool Success;
        public string Ex;

        public static LogoutResultDto Ok(string message = "Выход выполнен успешно")
        {
            return new LogoutResultDto { Success = true, Ex = message };
        }

        public static LogoutResultDto Fail(string message)
        {
            return new LogoutResultDto { Success = false, Ex = message };
        }
    }
}
