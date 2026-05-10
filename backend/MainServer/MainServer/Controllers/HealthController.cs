using MainServer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MainServer.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly DbContext _dbContext;

        public HealthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckServices()
        {
            try
            {
                bool canConnect = await _dbContext.Database.CanConnectAsync();
                if (!canConnect)
                    return StatusCode(503, new { Status = "Подключение к базе данных отсутсвует" });
                return Ok(new { Status = "Сервер работает и подключение к базе данных успешно" });
            }
            catch
            {
                return StatusCode(500, new { Status = "Внутренняя ошибка сервера" });
            }
        }
    }
}
