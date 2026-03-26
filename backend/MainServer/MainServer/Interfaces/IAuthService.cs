using SharedLib.ServerClasses;

namespace MainServer.Interfaces
{
    public interface IAuthService
    {
        public Task<RegisterResultDto> RegisterAsync(RegisterRequestDto request);
        public Task<LoginResultDto> LoginAsync(LoginRequestDto request);
        public Task<GetMeResultDto> GetMeAsync(int id);
    }//Интерфес описывает логику класса сервиса. Все методы асинхронные потому что они все будут выполнять запросы к БД.
}
