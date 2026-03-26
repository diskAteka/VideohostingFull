namespace SharedLib.DTOmodels
{
    public class CommentDto
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
