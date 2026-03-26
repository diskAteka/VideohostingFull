using System.ComponentModel.DataAnnotations;

namespace SharedLib.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int VideoId { get; set; } 
        public int UserId { get; set; }  

        [Required]
        [MaxLength(1000)] 
        public string Text { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public Video Video { get; set; }
        public User User { get; set; }
    }
}
