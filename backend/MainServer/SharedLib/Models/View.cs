namespace SharedLib.Models
{
    public class View
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public int UserId { get; set; }


        // Навигационные свойства
        public Video Video { get; set; }
        public User User { get; set; }
    }
}
