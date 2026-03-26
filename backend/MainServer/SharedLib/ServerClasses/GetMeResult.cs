using SharedLib.Models;

namespace SharedLib.ServerClasses
{
    public class GetMeResultDto
    {
        public bool Success {  get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool CanUpload { get; set; }
        public bool IsActive { get; set; }

        public static GetMeResultDto Ok(GetMeResultDto me,string message = "Получение данных пользователя выполнено успешно")
        {
            return new GetMeResultDto { Success = true, Message = message, Name = me.Name, Email = me.Email, CanUpload = me.CanUpload, IsActive = me.IsActive };
        }

        public static GetMeResultDto Fail(string message)
        {
            return new GetMeResultDto { Success = false, Message = message };
        }
    }
}
