using MainServer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOmodels;
using SharedLib.DTOmodels.RequestModel;
using SharedLib.GlobalClasses;
using SharedLib.GlobalInterfaces;
using SharedLib.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace MainServer.Controllers
{
    [ApiController]
    [Route("api/videos")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly ILogger<VideoController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public VideoController(IHttpClientFactory httpClientFactory, IVideoService videoService, ILogger<VideoController> logger, ITokenService tokenService)
        {
            _httpClientFactory = httpClientFactory;
            _videoService = videoService;
            _logger = logger;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVideo()
        {
            try
            {
                List<VideoListItemDto> videos = await _videoService.GetAllVideoAsync();
                return Ok(videos);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при отправке всех видео");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new { message = ex.Message });
            }
        }//Метод отпправки всех видео клиенту

        [HttpGet("search")]
        public async Task<IActionResult> SearchVideo([FromQuery] string query)
        {
            try
            {
                List<VideoListItemDto> videos = await _videoService.VideoSearchAsync(query);
                return Ok(videos);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при отправке результата поиска");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new { message = ex.Message });
            }
        }//Отправляет список из 10 видео по запросу

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetVideo(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                VideoDetailDto video = await _videoService.GetVideoAsync(id, userId);
                return Ok(video);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при отправке видео");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new { message = ex.Message });
            }
        }//Отправляет данные видео

        [HttpPost("{id}/reaction")]
        [Authorize]
        public async Task<IActionResult> Reaction(int id, [FromBody] ReactionDto reaction )
        {
            try
            {
                int videoId = id;
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                ReactionRequest request = new ReactionRequest { UserId = userId, IsLike = reaction.IsLike, VideoId = videoId };
                await _videoService.ReactionAsync(request);
                return Ok();
            }
            catch(ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при получении реакции");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new { message = ex.Message });
            }
        }//Обрабатывет реакцию пользователя

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetVideoComment(int id)
        {
            try
            {
                List<CommentDto> comments = await _videoService.GetCommentsAsync(id);
                return Ok(comments);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при получении комменатриев");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new { message = ex.Message });
            }
        }//Отправляет список комментариев к видео

        [HttpPost("{id}/comments")]
        [Authorize]
        public async Task<IActionResult> NewComment([FromBody]  CommentMinDto comment, int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                CommentRequest request = new()
                {
                    UserId = userId,
                    Text = comment.Text,
                    VideoId = id
                };

                await _videoService.NewCommentAsync(request);
                return Ok(request);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при создании комментария");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new { message = ex.Message });
            }
        }//Создает новый комментарий

        [HttpGet("{id}/stream")]
        public async Task<IActionResult> StreamVideo(int id)
        {
            try
            {
                var video = await _videoService.GetVideoMetadataAsync(id);

                if (video == null)
                    return NotFound("Видео не найдено");

                if (!video.IsVerified) return Forbid();

                string videoUrl = video.Link;

                HttpClient client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(videoUrl, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode)
                    return NotFound("Файл с видео не найден");

                var stream = await response.Content.ReadAsStreamAsync();

                foreach ( var header in response.Headers)
                    Response.Headers.TryAdd(header.Key, header.Value.ToArray());

                foreach (var header in response.Content.Headers)
                    Response.Headers.TryAdd(header.Key, header.Value.ToArray());

                return File(stream, response.Content.Headers.ContentType?.MediaType ?? "video/mp4", enableRangeProcessing: true);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Ошибка при стриминге видео");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при стриминге видео");
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }//Создает стриминговый поток


        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadVideo([FromForm] VideoUploadRequest request)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                int newVideoId = await _videoService.NewVideoAsync(request, userId);
                VideoDetailDto video = await _videoService.GetVideoAsync(newVideoId, userId);
                return Ok(video);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при создании видео");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new { message = ex.Message });
            }
        }

        [HttpGet("upload")]
        [Authorize]
        public async Task<IActionResult> GetUserVideos()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                List<UserVideoListItemDto> videos = await _videoService.GetThisUserVideos(userId);
                return Ok(videos);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка контроллера при получении видео");
                return StatusCode(ApiException.GetStatusCode(ex.ErrorType), new { message = ex.Message });
            }
        }
    }
}
