using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using MainServer.Interfaces;
using SharedLib.DTOmodels.RequestModel;
using SharedLib.Models;
using System.Runtime.CompilerServices;

namespace MainServer.Services
{
    public class StorageService : IStorageService
    {
        private readonly string AccessKey;
        private readonly string AccessKeySecret;
        private readonly string GarageEndpoint;
        private readonly string _videoBasePath;
        private readonly string _posterBasePath;
        private readonly string _tempPath;
        private readonly string _bucketName;
        private readonly PosterService _posterService;
        private readonly ILogger<StorageService> _logger;
        private readonly IAmazonS3 _s3Client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StorageService(IConfiguration configuration, PosterService posterService, 
            ILogger<StorageService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _posterService = posterService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            AccessKey = configuration["Garage:AccessKey"];
            AccessKeySecret = configuration["Garage:SecretKey"];
            GarageEndpoint = configuration["Garage:Endpoint"];
            _videoBasePath = configuration["Garage:VideoBasePath"];
            _posterBasePath = configuration["Garage:PosterBasePath"];
            _bucketName = configuration["Garage:BucketName"];
            _tempPath = configuration["VideoStorage"] ?? "/MainServer/temp_videos";

            Directory.CreateDirectory(_tempPath);

            _s3Client = new AmazonS3Client(
                awsAccessKeyId: AccessKey,
                awsSecretAccessKey: AccessKeySecret,
                new AmazonS3Config
                {
                    ServiceURL = GarageEndpoint,
                    ForcePathStyle = true,
                    MaxErrorRetry = 3,
                    Timeout = TimeSpan.FromSeconds(30)
                });
            
        }

        public async Task<Video> UploadToStorageAsync(VideoUploadRequest request, int authorId, string tempFilePath)
        {
            if (!File.Exists(tempFilePath))
                throw new FileNotFoundException($"Видео по указанному пути не найдено: {tempFilePath}");

            string videoExtension = Path.GetExtension(tempFilePath);
            string videoObjectKey = $"{_videoBasePath}/{Guid.NewGuid()}{videoExtension}";

            await UploadFileAsync(tempFilePath, videoObjectKey, GetContentType(videoExtension));

            string posterPath = await _posterService.ExtractPoster(tempFilePath);
            string posterExtension = Path.GetExtension(posterPath);
            string posterObjectKey = $"{_posterBasePath}/{Guid.NewGuid()}{posterExtension}";

            try
            {
                await UploadFileAsync(
                    posterPath,
                    posterObjectKey,
                    GetContentType(posterExtension));

                await VerifyObjectExistsAsync(videoObjectKey);
            }
            finally
            {
                if (File.Exists(posterPath))
                {
                    File.Delete(posterPath);
                }
            }

            Video newVideo = new()
            {
                Name = request.Title,
                Description = request.Description,
                DateUpload = DateTime.UtcNow,
                Link = BuildObjectUrl(videoObjectKey),
                Poster = BuildObjectUrl(posterObjectKey),
                Likes = 0,
                Dislikes = 0,
                Views = 0,
                IsVerified = false,
                AuthorId = authorId,
            };

            return newVideo;
        }

        private async Task UploadFileAsync(string filePath, string objectKey, string contentType)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                FilePath = filePath,
                ContentType = contentType
            };

            var response = await _s3Client.PutObjectAsync(request);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(
                    $"Не удалось загрузить {objectKey}. Статус: {response.HttpStatusCode}");
            }

            _logger.LogInformation("Uploaded {ObjectKey} to bucket {BucketName}" ,objectKey, _bucketName);
        }

        private async Task VerifyObjectExistsAsync(string objectKey)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = objectKey
                };

                await _s3Client.GetObjectMetadataAsync(request);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception(
                    $"Upload verification failed: object {objectKey} not found", ex);
            }
        }

        private string BuildObjectUrl(string objectKey)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            var encodedKey = Uri.EscapeDataString(objectKey);
            return $"{baseUrl}/api/storage/file/{encodedKey}";
        }

        private string GetContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".mp4" => "video/mp4",
                ".webm" => "video/webm",
                ".avi" => "video/x-msvideo",
                ".mov" => "video/quicktime",
                _ => "application/octet-stream"
            };
        }

        public async Task DeleteVideoFilesAsync(string videoKey, string posterKey)
        {
            var keys = new List<KeyVersion>
            {
                new() { Key = videoKey },
                new() { Key = posterKey }
            };

            var request = new DeleteObjectsRequest
            {
                BucketName = _bucketName,
                Objects = keys
            };

            var response = await _s3Client.DeleteObjectsAsync(request);

            foreach (var error in response.DeleteErrors)
            {
                _logger.LogError(
                    "Failed to delete {Key}: {Message}",
                    error.Key,
                    error.Message);
            }
        }

        public async Task<string> GeneratePresignedUrlAsync(string objectKey, int expiryHours = 1)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddHours(expiryHours)
            };

            return await Task.FromResult(_s3Client.GetPreSignedURL(request));
        }
    }
}
