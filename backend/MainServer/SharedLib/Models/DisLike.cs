namespace SharedLib.Models
{
    public class DisLike
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public int UserId { get; set; }


        // Навигационные свойства
        public virtual Video Video { get; set; }
        public virtual User User { get; set; }
    }
}
