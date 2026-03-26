using SharedLib.Enums;

namespace SharedLib.Models
{
    public class ServerLog
    {
        public int Id { get; set; }

        public int UserId { get; set; } 

        public int Type { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow; 

        public User User { get; set; }
    }
}
