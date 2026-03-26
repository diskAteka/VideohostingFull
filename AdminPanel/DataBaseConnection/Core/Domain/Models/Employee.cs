using DataBaseConnection.Core.Domain.Interfaces;


namespace DataBaseConnection.Core.Domain.Models;

public partial class Employee : IAddble, IModel, IUpdateble , IDeleteble
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;
}
