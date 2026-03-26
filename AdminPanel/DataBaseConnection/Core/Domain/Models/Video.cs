using DataBaseConnection.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace DataBaseConnection.Core.Domain.Models;

public partial class Video : IDeleteble, IUpdateble, IAddble, IModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime DateUpload { get; set; }

    public string Link { get; set; } = null!;

    public string Poster { get; set; } = null!;

    public int Likes { get; set; }

    public int Dislikes { get; set; }

    public bool IsVerified { get; set; }

    public int Views { get; set; }

    public int AuthorId { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<DisLike> DisLikes { get; set; } = new List<DisLike>();

    public virtual ICollection<Like> LikesNavigation { get; set; } = new List<Like>();

    public virtual ICollection<View> ViewsNavigation { get; set; } = new List<View>();
}
