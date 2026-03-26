using DataBaseConnection.Core.Domain.Interfaces;

namespace DataBaseConnection.Core.Domain.Models;

public partial class ServerLog : IModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Type { get; set; } = null!;

    public DateTime Date { get; set; }

    public virtual User User { get; set; } = null!;
}
