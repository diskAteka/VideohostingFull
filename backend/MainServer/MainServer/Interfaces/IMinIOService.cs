using SharedLib.DTOmodels.RequestModel;
using SharedLib.Models;

namespace MainServer.Interfaces
{
    public interface IMinIOService
    {
        Task<Video> UploadToMinIOAsync(VideoUploadRequest request, int AuthorId, string FileName);
    }
}
