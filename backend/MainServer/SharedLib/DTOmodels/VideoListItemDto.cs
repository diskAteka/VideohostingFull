namespace SharedLib.DTOmodels
{
    public class VideoListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Poster { get; set; }
        public DateTime DateUpload { get; set; }
        public string AuthorName {  get; set; }

    }
    public class UserVideoListItemDto : VideoListItemDto
    {
        public bool IsVerified { get; set; }
    }
}
