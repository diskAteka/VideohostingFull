using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLib.Models
{
    public class Video
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        public DateTime DateUpload { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(500)]
        [Url] 
        public string Link { get; set; }
        [Required]
        [MaxLength(500)]
        [Url]
        public string Poster { get; set; }

        public int Likes { get; set; }
        public int Dislikes { get; set; }

        [Column(TypeName = "bit")] 
        public bool IsVerified { get; set; } 

        public int Views { get; set; }

        public int AuthorId { get; set; } 

        // Навигационные свойства
        public User Author { get; set; }

        // Обратная связь к комментариям
        public virtual ICollection<Comment> Comments { get; set; } = [];

        // Обратная связь с лайками
        public virtual ICollection<Like> LikesTable { get; set; } = [];

        // Обратная связь с дизлайками
        public virtual ICollection<DisLike> DisLikesTable { get; set; } = [];

        // Обратная связь с просмотрами
        public virtual ICollection<View> ViewsTable { get; set; } = [];



        // Вычисляемые свойства (не хранятся в БД)
        public string VideoUrl => $"content/videos/{Link}";
        public string PosterUrl => $"content/posters/{Poster}";

    }
}
