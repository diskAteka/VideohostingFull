using SharedLib.DTOmodels.RequestModel;
using SharedLib.Models;

namespace MainServer.Interfaces
{
    public interface IStorageService
    {
        Task<Video> UploadToStorageAsync(VideoUploadRequest request, int AuthorId, string FileName);
    }
}
