using Microsoft.Extensions.Hosting.Internal;
using Xabe.FFmpeg;


namespace MainServer.Services
{
    public class PosterService
    {
        string outputDirectory;
        public PosterService(IConfiguration configuration)
        {
            outputDirectory = configuration["VideoStorage:TempPath"] ?? "/MainServer/temp_videos";
        }

        public async Task<string> ExtractPoster(string videoPath)
        {
            try
            {
                if (!File.Exists(videoPath))
                    throw new FileNotFoundException();

                var fileName = $"{outputDirectory}/poster_{DateTime.Now.Ticks}.jpg";

                var videoinfo = FFmpeg.GetMediaInfo(videoPath);

                var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(
                    videoPath,
                    fileName,
                    TimeSpan.FromSeconds(5)
                );
                var rezult = await conversion.Start();

                return fileName;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ошибка при создании постера, было использовано постер по умолчанию");
                return @"Recurce\placeholder-256.png";
            }            

        }
    }
}
