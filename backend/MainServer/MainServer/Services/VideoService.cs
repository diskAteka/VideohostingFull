using MainServer.Data;
using MainServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SharedLib.DTOmodels;
using SharedLib.DTOmodels.RequestModel;
using SharedLib.Enums;
using SharedLib.GlobalClasses;
using SharedLib.GlobalInterfaces;
using SharedLib.Models;
using System.Data.Common;
using View = SharedLib.Models.View;


namespace MainServer.Services
{
    public class VideoService : IVideoService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VideoService> _logger;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;
        private readonly IMinIOService _minIOService;
        private readonly IConfiguration _configuration;

        public VideoService(AppDbContext context, 
            ILogger<VideoService> logger, 
            ITokenService tokenService, 
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment environment,
            IMinIOService minIOService,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
            _minIOService = minIOService;
            _configuration = configuration;
        }

        public async Task<List<VideoListItemDto>> GetAllVideoAsync()
        {
            try
            {
                List<VideoListItemDto> videoList = await _context.Videos
                    .AsNoTracking()
                    .Include(v => v.Author)
                    .OrderByDescending(v => v.Views)
                    .Where(v => v.IsVerified)
                    .Take(20)
                    .Select(v => new VideoListItemDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Poster = v.PosterUrl,
                        DateUpload = v.DateUpload,
                        AuthorName = v.Author.Name
                    })
                    .ToListAsync();
                return videoList;
            }
            catch (DbException ex)
            {
                throw new ApiException(ErrorType.ServerError, $"Ошибка при чтении данных {ex.Message}");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError($"Неожиданная ошибка при чтении данных видео, Exeption {ex.Message}");
                throw new ApiException(ErrorType.ServerError, "Внутренняя ошибка сервера");
            }
        }//Возвращает коллекцию из максимум 20 видео или возвращает null

        public async Task<VideoDetailDto> GetVideoAsync(int videoId, int userId)
        {
            try
            {
                bool isViewed = await _context.Views.FirstOrDefaultAsync(v => (v.UserId == userId) && (v.VideoId == videoId)) == null ? false : true;
                if (!isViewed)
                {
                    await _context.AddAsync(new View { UserId = userId, VideoId = videoId });
                    await _context.Videos.Where(v => v.Id == videoId)
                        .ExecuteUpdateAsync(s => s.SetProperty(v => v.Views, v => v.Views + 1));

                    await _context.SaveChangesAsync();
                }


                Video v = await _context.Videos
                    .AsNoTracking()
                    .Include(v => v.Author)
                    .Include(v => v.Comments)
                    .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(v => v.Id == videoId);
                ICollection<Comment> c = v.Comments;

                //Если в таблице с лайками есть запись с этим пользователем и с этим видео, значит этот лайкнул посмотрел это видео. 
                //Аналогично для других флагов
                bool isLiked = await _context.Likes.FirstOrDefaultAsync(l => (l.UserId == userId) && (l.VideoId == videoId)) == null ? false : true;
                bool isDisliked = await _context.DisLikes.FirstOrDefaultAsync(d => (d.UserId == userId) && (d.VideoId == videoId)) == null ? false : true;
                List<CommentDto> Comments = c.Select(c => new CommentDto
                {
                    AuthorId = c.UserId,
                    AuthorName = c.User.Name,
                    Text = c.Text,
                    CreatedAt = c.Date
                }).ToList();

                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

                VideoDetailDto videoDto = new()
                {
                    Id = v.Id,
                    Name = v.Name,
                    Description = v.Description,
                    DateUpload = v.DateUpload,
                    Link = v.Link,
                    Poster = v.Poster,
                    Views = v.Views,
                    Likes = v.Likes,
                    Dislikes = v.Dislikes,
                    IsLiked = isLiked,
                    IsDisLiked = isDisliked,
                    Comments = Comments,

                    VideoUrl = $"{baseUrl}/{v.VideoUrl}",
                    PosterUrl = $"{baseUrl}/{v.PosterUrl}"

                };

                return videoDto;
            }
            catch (DbException ex)
            {
                throw new ApiException(ErrorType.ServerError, $"Ошибка при чтении данных {ex.Message}");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError($"Неожиданная ошибка при чтении данных видео, Exeption {ex.Message}");
                throw new ApiException(ErrorType.ServerError, "Внутренняя ошибка сервера");
            }
        }//Возвращает video по id или возвращает null

        public async Task<List<VideoListItemDto>> VideoSearchAsync(string query, int limit = 10)
        {
            try
            {
                var searchLower = query.ToLower().Trim();

                if (searchLower.Length < 2)
                    return new List<VideoListItemDto>();

                // Получаем все видео сначала
                var videoQuery = _context.Videos
                    .Where(v => v.IsVerified)
                    .Include(v => v.Author)
                    .Where(v => v.Name.ToLower().Contains(searchLower) ||
                               v.Description.ToLower().Contains(searchLower));

                // Выполняем в памяти для сложной сортировки
                var videos = await videoQuery.ToListAsync();

                var sortedVideos = videos
                    .Select(v => new
                    {
                        Video = v,
                        Score = CalculateRelevanceScore(v, searchLower)
                    })
                    .OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.Video.Views)
                    .Take(limit)
                    .Select(x => new VideoListItemDto
                    {
                        Id = x.Video.Id,
                        Name = x.Video.Name,
                        Poster = x.Video.PosterUrl,
                        DateUpload = x.Video.DateUpload,
                        AuthorName = x.Video.Author.Name
                    })
                    .ToList();

                return sortedVideos;
            }
            catch (DbException ex)
            {
                throw new ApiException(ErrorType.ServerError, $"Ошибка при поиске видео: {ex.Message}");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError(ex, "Неожиданная ошибка при поиске видео, запрос: {Query}", query);
                throw new ApiException(ErrorType.ServerError, "Ошибка при поиске");
            }
        }//Получение массива с данными о 10 видео наиболее соответсвующих

        public async Task NewCommentAsync(CommentRequest request)
        {
            try
            {
                var videoExists = await _context.Videos.AnyAsync(v => v.Id == request.VideoId);
                if (!videoExists)
                    throw new ApiException(ErrorType.NotFound, $"Видео {request.VideoId} не найдено");
                if (string.IsNullOrWhiteSpace(request.Text) || request.Text.Length > 1000)
                    throw new ApiException(ErrorType.ValidationError, "Комментарий должен содержать 1-1000 символов");


                Comment comment = new();
                comment.VideoId = request.VideoId;
                comment.UserId = request.UserId;
                comment.Text = request.Text;
                comment.Date = DateTime.Now;

                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new ApiException(ErrorType.ServerError, $"Ошибка при сохранении в БД {ex.Message}");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError($"Неожиданная ошибка при добавлении комментария, Exeption {ex.Message}");
                throw new ApiException(ErrorType.ServerError, "Внутренняя ошибка сервера");
            }
        }//Добавляет новый коментарий в БД

        public async Task<List<CommentDto>> GetCommentsAsync(int videoId)
        {
            try
            {
                bool videoExists = await _context.Videos.AnyAsync(v => v.Id == videoId);
                if (!videoExists)
                    throw new ApiException(ErrorType.NotFound, $"Видео {videoId} не найдено");

                List<CommentDto> comments = await _context.Comments
                    .AsNoTracking()
                    .Where(c => c.VideoId == videoId)
                    .Include(c => c.User)
                    .Select(c => new CommentDto
                    {
                        AuthorId = c.UserId,
                        AuthorName = c.User.Name,
                        CreatedAt = c.Date,
                        Text = c.Text
                    })
                    .OrderBy(c => c.CreatedAt)
                    .ToListAsync();
                
                return comments;
            }
            catch (DbUpdateException ex)
            {
                throw new ApiException(ErrorType.ServerError, $"Ошибка при запросе в БД {ex.Message}");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError($"Неожиданная ошибка при запросе комментарив, Exeption {ex.Message}");
                throw new ApiException(ErrorType.ServerError, "Внутренняя ошибка сервера");
            }
        }//Получает список комментариев к видео

        public async Task ReactionAsync(ReactionRequest request)
        {
            bool IsLike = request.IsLike;
            int userId = request.UserId;
            int videoId = request.VideoId;
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Like existLike = await _context.Likes.FirstOrDefaultAsync(l => (l.UserId == userId) && (l.VideoId == videoId));
                DisLike existDislike = await _context.DisLikes.FirstOrDefaultAsync(d => (d.UserId == userId) && (d.VideoId == videoId));

                if (IsLike)
                {
                    if (existLike == null)
                    {
                        if (existDislike == null)
                        {
                            await AddLike(userId, videoId);
                        }
                        else
                        {
                            await DeleteDislike(existDislike, videoId);
                            await AddLike(userId, videoId);
                        }
                    }
                    else
                    {
                        await DeleteLike(existLike, videoId);
                    }
                }
                else
                {
                    if (existDislike == null)
                    {
                        if (existLike == null)
                        {
                            await AddDislike(userId, videoId);
                        }
                        else
                        {
                            await DeleteLike(existLike, videoId);
                            await AddDislike(userId, videoId);
                        }
                    }
                    else
                    {
                        await DeleteDislike(existDislike, videoId);
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }//Обрабатывает реакцию пользователя

        public async Task<Video> GetVideoMetadataAsync(int videoId)
        {
            try
            {
                return await _context.Videos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == videoId);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, $"Ошибка БД при получении метаданных видео {videoId}");
                throw new ApiException(ErrorType.ServerError, "Ошибка при получении данных видео");
            }
        }//Получение метаданных видео

        public async Task<int> NewVideoAsync(VideoUploadRequest request, int AuthorId)
        {
            if (request == null)
                throw new ApiException(ErrorType.ValidationError, "Запрос не может быть пустым");

            if (request.Video == null || request.Video.Length == 0)
                throw new ApiException(ErrorType.ValidationError, "Файл видео не выбран");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ApiException(ErrorType.ValidationError, "Название видео обязательно");

            string safeTitle = string.Join("_", request.Title.Split(Path.GetInvalidFileNameChars()));

            var extension = Path.GetExtension(request.Video.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".mp4" };

            if (!allowedExtensions.Contains(extension))
                throw new ApiException(ErrorType.ValidationError,
                    $"Неподдерживаемый формат. Разрешены: {string.Join(", ", allowedExtensions)}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == AuthorId);
            if (user == null)
                throw new ApiException(ErrorType.NotFound, "Пользователь не найден");

            if (!user.CanUpload)
                throw new ApiException(ErrorType.Forbidden, "У вас нет прав на загрузку видео");

            const long maxFileSize = 2L * 1024 * 1024 * 1024;
            if (request.Video.Length > maxFileSize)
                throw new ApiException(ErrorType.Conflict,
                    $"Файл слишком большой. Максимальный размер 2GB");

            string tempDir = _configuration["VideoStorage:TempPath"] ?? "/MainServer/temp_videos";
            Directory.CreateDirectory(tempDir);

            string tempFileName = $"{Guid.NewGuid()}{extension}";
            string tempFilePath = Path.Combine(tempDir, tempFileName);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                using (var stream = new FileStream(tempFilePath, FileMode.CreateNew))
                    await request.Video.CopyToAsync(stream);

                Video newVideo = await _minIOService.UploadToMinIOAsync(request, AuthorId, tempFilePath);




                await _context.Videos.AddAsync(newVideo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                File.Delete(tempFilePath);

                return newVideo.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                if (File.Exists(tempFilePath))
                {
                    try { File.Delete(tempFilePath); } catch {}
                }

                if (ex is DbUpdateException)
                    throw new ApiException(ErrorType.ServerError, $"Ошибка при сохранении в БД: {ex.Message}");
                else if (ex is ApiException)
                    throw;
                else
                {
                    _logger.LogError(ex, "Неожиданная ошибка при добавлении видео");
                    throw new ApiException(ErrorType.ServerError, "Внутренняя ошибка сервера");
                }
            }
        }

        public async Task<List<UserVideoListItemDto>> GetThisUserVideos(int userId)
        {
            try
            {
                List<UserVideoListItemDto> videoList = await _context.Videos
                    .AsNoTracking()
                    .Include(v => v.Author)
                    .OrderByDescending(v => v.Views)
                    .Where(v => v.AuthorId == userId)
                    .Select(v => new UserVideoListItemDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Poster = v.PosterUrl,
                        DateUpload = v.DateUpload,
                        AuthorName = v.Author.Name,
                        IsVerified = v.IsVerified
                    })
                    .ToListAsync();
                return videoList;
            }
            catch (DbException ex)
            {
                throw new ApiException(ErrorType.ServerError, $"Ошибка при чтении данных {ex.Message}");
            }
            catch (Exception ex) when (ex is not ApiException)
            {
                _logger.LogError($"Неожиданная ошибка при чтении данных видео, Exeption {ex.Message}");
                throw new ApiException(ErrorType.ServerError, "Внутренняя ошибка сервера");
            }
        }

        private async Task DeleteLike(Like like, int videoId)
        {
            _context.Remove(like);
            await _context.Videos.Where(v => v.Id == videoId).ExecuteUpdateAsync(s => s.SetProperty(v => v.Likes, v => v.Likes - 1));
        }

        private async Task AddLike(int userId, int videoId)
        {
            await _context.Likes.AddAsync(new Like { UserId = userId, VideoId = videoId });
            await _context.Videos.Where(v => v.Id == videoId).ExecuteUpdateAsync(s => s.SetProperty(v => v.Likes, v => v.Likes + 1));
        }

        private async Task DeleteDislike(DisLike dislike, int videoId)
        {
            _context.Remove(dislike);
            await _context.Videos.Where(v => v.Id == videoId).ExecuteUpdateAsync(s => s.SetProperty(v => v.Dislikes, v => v.Dislikes - 1));
        }

        private async Task AddDislike(int userId, int videoId)
        {
            await _context.DisLikes.AddAsync(new DisLike { UserId = userId, VideoId = videoId });
            await _context.Videos.Where(v => v.Id == videoId).ExecuteUpdateAsync(s => s.SetProperty(v => v.Dislikes, v => v.Dislikes + 1));
        }

        private int CalculateRelevanceScore(Video video, string searchLower)
        {
            int score = 0;
            var nameLower = video.Name.ToLower();
            var descLower = video.Description?.ToLower() ?? "";

            // Точное совпадение названия
            if (nameLower == searchLower) score += 200;

            // Начинается с поисковой строки
            if (nameLower.StartsWith(searchLower)) score += 100;

            // Слово начинается с поисковой строки
            var words = nameLower.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Any(word => word.StartsWith(searchLower))) score += 80;

            // Содержится в названии (чем раньше - тем лучше)
            var index = nameLower.IndexOf(searchLower);
            if (index >= 0) score += Math.Max(0, 50 - index); // Максимум 50 баллов

            // Содержится в описании
            if (descLower.Contains(searchLower)) score += 30;

            return score;
        }


    }
}
