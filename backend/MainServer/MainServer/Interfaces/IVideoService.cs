using SharedLib.DTOmodels;
using SharedLib.DTOmodels.RequestModel;
using SharedLib.Models;

namespace MainServer.Interfaces
{
    public interface IVideoService
    {
        public Task<List<VideoListItemDto>> GetAllVideoAsync();
        public Task<VideoDetailDto> GetVideoAsync(int videoId, int userId);
        public Task<List<VideoListItemDto>> VideoSearchAsync(string query, int limit = 10);
        public Task NewCommentAsync(CommentRequest request);
        public Task ReactionAsync(ReactionRequest request);
        public Task<List<CommentDto>> GetCommentsAsync(int videoId);
        public Task<Video> GetVideoMetadataAsync(int videoId);
        public Task<int> NewVideoAsync(VideoUploadRequest request, int AuthorId);
        public Task<List<UserVideoListItemDto>> GetThisUserVideos(int userId);

    }
}
