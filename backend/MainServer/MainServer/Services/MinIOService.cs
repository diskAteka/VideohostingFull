using MainServer.Interfaces;
using Minio;
using Minio.DataModel.Args;
using SharedLib.DTOmodels.RequestModel;
using SharedLib.Models;

namespace MainServer.Services
{
    public class MinIOService : IMinIOService
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogger<MinIOService> _logger;
        private readonly PosterService _posterService;
        private readonly string _VideoBucketName;
        private readonly string _PosterBucketName;
        private readonly string _tempPath;
        private readonly string _endpoint;

        public MinIOService(IConfiguration configuration, ILogger<MinIOService> logger, PosterService posterService)
        {
            _posterService = posterService;
            _logger = logger;
            _endpoint = configuration["MinIO:Endpoint"];
            _VideoBucketName = configuration["MinIO:VideoBucketName"] ?? "videos";
            _PosterBucketName = configuration["MinIO:PosterBucketName"] ?? "posters";
            _tempPath = configuration["VideoStorage:TempPath"] ?? "/MainServer/temp_videos";

            Directory.CreateDirectory(_tempPath);

            _minioClient = new MinioClient()
                .WithEndpoint(configuration["MinIO:Endpoint"])
                .WithCredentials(
                    configuration["MinIO:AccessKey"],
                    configuration["MinIO:SecretKey"]
                )
                .WithSSL(false)
                .Build();

            Task.Run(EnsureBucketsExistsAsync).Wait();
        }

        public async Task<Video> UploadToMinIOAsync(VideoUploadRequest request, int authorId, string tempFilePath)
        {
            if (!File.Exists(tempFilePath))
                throw new Exception($"Видео по указанному пути не найдено: {tempFilePath}");

            // Убеждаемся, что оба bucket существуют
            await EnsureBucketExistsAsync(_VideoBucketName);
            await EnsureBucketExistsAsync(_PosterBucketName);

            // 1. Загружаем видео
            string videoExtension = Path.GetExtension(tempFilePath);
            string videoObjectName = $"{Guid.NewGuid()}{videoExtension}";

            await using (var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read))
            {
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_VideoBucketName)
                    .WithObject(videoObjectName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(GetContentType(videoExtension));

                await _minioClient.PutObjectAsync(putObjectArgs);
            }

            // 2. Создаем и загружаем постер
            string posterPath = await _posterService.ExtractPoster(tempFilePath);
            string posterExtension = Path.GetExtension(posterPath);
            string posterObjectName = $"{Guid.NewGuid()}{posterExtension}";

            await using (var fileStream = new FileStream(posterPath, FileMode.Open, FileAccess.Read))
            {
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_PosterBucketName)
                    .WithObject(posterObjectName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(GetContentType(posterExtension));

                await _minioClient.PutObjectAsync(putObjectArgs);
            }

            // 3. Удаляем временный постер
            File.Delete(posterPath);

            // 4. Создаем запись в БД
            Video newVideo = new()
            {
                Name = request.Title,
                Description = request.Description,
                DateUpload = DateTime.UtcNow,
                Link = $"http://{_endpoint}/{_VideoBucketName}/{videoObjectName}",
                Poster = $"http://{_endpoint}/{_PosterBucketName}/{posterObjectName}",
                Likes = 0,
                Dislikes = 0,
                Views = 0,
                IsVerified = false,
                AuthorId = authorId,
            };

            return newVideo;
        }//Отправляет видео в облачное хранилтще

        private async Task EnsureBucketsExistsAsync()
        {
            await EnsureBucketExistsAsync(_PosterBucketName);
            await EnsureBucketExistsAsync(_VideoBucketName);
        }

        private async Task EnsureBucketExistsAsync(string backet)
        {
            try
            {
                var exists = await _minioClient.BucketExistsAsync(
                    new BucketExistsArgs().WithBucket(backet));

                if (!exists)
                {
                    await _minioClient.MakeBucketAsync(
                        new MakeBucketArgs().WithBucket(backet));
                    _logger.LogInformation("Created bucket: {Bucket}", backet);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ensure bucket exists");
                throw;
            }
        }

        private string GetContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".mp4" => "video/mp4",
                _ => "application/octet-stream"
            };
        }
    }
}

