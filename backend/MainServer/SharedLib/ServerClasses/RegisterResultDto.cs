using SharedLib.Models;

namespace SharedLib.ServerClasses
{
    public class RegisterResultDto
    {
        public bool Success { get; set; }
        public string Message {  get; set; }
        public static RegisterResultDto Ok(string ex = "Регистрация выполнена успешно")
        {
            RegisterResultDto dto = new RegisterResultDto
            {
                Success = true,
                Message = ex
            };
            return dto;
        }
    }
}
