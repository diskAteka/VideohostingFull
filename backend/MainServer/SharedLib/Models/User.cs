using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLib.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(64)] // Для хэша SHA256 (32 байта в hex)
        [Column(TypeName = "char(64)")]
        public string PasswordHash { get; set; } // Было: Password

        [Required]
        [MaxLength(254)] // Максимальная длина email по стандарту
        [EmailAddress]
        public string Email { get; set; }

        public bool CanUpload { get; set; }

        // Навигационные свойства - используем ICollection для гибкости
        public virtual ICollection<Video> Videos { get; set; } = [];
        public virtual ICollection<Comment> Comments { get; set; } = [];
        public virtual ICollection<ServerLog> ServerLogs { get; set; } = [];
        public virtual ICollection<Like> LikesTable { get; set; } = [];
        public virtual ICollection<DisLike> DisLikesTable { get; set; } = [];
        public virtual ICollection<View> ViewsTable { get; set; } = [];

        // Добавляем соль для хэширования пароля
        [MaxLength(32)]
        [Column(TypeName = "char(32)")]
        public string PasswordSalt { get; set; }

        // Дата регистрации для аналитики
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Флаг активности пользователя
        public bool IsActive { get; set; } = true;



    }
}
