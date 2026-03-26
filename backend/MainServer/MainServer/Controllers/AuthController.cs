using MainServer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.GlobalClasses;
using SharedLib.GlobalInterfaces;
using SharedLib.ServerClasses;
using System.Security.Claims;

namespace MainServer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, ITokenService tokenService)
        {
            _authService = authService;
            _logger = logger;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                RegisterResultDto result = await _authService.RegisterAsync(request);//Сервис сам выбросит исключение если произойдет ошибка
                return Ok(result);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при регистрации");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType) , new {message = ex.Message});
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                LoginResultDto rezult = await _authService.LoginAsync(request);
                return Ok(rezult);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при регистрации");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new {message = ex.Message});
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                GetMeResultDto rezult = await _authService.GetMeAsync(id);
                return Ok(rezult);
            }
            catch(ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при получении данных пользователя");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new {message = ex.Message});
            }

        }
    }
}
