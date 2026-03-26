using DataBaseConnection.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace DataBaseConnection.Core.Domain.Models;

public partial class Like : IAddble, IModel, IDeleteble
{
    public int Id { get; set; }

    public int VideoId { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Video Video { get; set; } = null!;
}
