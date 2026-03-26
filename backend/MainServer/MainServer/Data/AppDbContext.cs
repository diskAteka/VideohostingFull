using System;
using System.Collections.Generic;
using SharedLib.Models;
using Microsoft.EntityFrameworkCore;

namespace MainServer.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<DisLike> DisLikes { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<ServerLog> ServerLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    public virtual DbSet<View> Views { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3214EC0757905008");

            entity.ToTable("Comment");

            entity.HasIndex(e => e.Date, "IX_Comment_Date");

            entity.HasIndex(e => new { e.VideoId, e.Date }, "IX_Comment_VideoId_Date");

            entity.Property(e => e.Text).HasMaxLength(1000);

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__UserId__59063A47");

            entity.HasOne(d => d.Video).WithMany(p => p.Comments)
                .HasForeignKey(d => d.VideoId)
                .HasConstraintName("FK__Comment__VideoId__5812160E");
        });

        modelBuilder.Entity<DisLike>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DisLike__3214EC071D73A606");

            entity.ToTable("DisLike");

            entity.HasIndex(e => new { e.VideoId, e.UserId }, "IX_DisLike_VideoId_UserId").IsUnique();

            entity.HasOne(d => d.User).WithMany(p => p.DisLikesTable)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DisLike__UserId__6477ECF3");

            entity.HasOne(d => d.Video).WithMany(p => p.DisLikesTable)
                .HasForeignKey(d => d.VideoId)
                .HasConstraintName("FK__DisLike__VideoId__6383C8BA");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Like__3214EC07888C3CD8");

            entity.ToTable("Like");

            entity.HasIndex(e => new { e.VideoId, e.UserId }, "IX_Like_VideoId_UserId").IsUnique();

            entity.HasOne(d => d.User).WithMany(p => p.LikesTable)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Like__UserId__60A75C0F");

            entity.HasOne(d => d.Video).WithMany(p => p.LikesTable)
                .HasForeignKey(d => d.VideoId)
                .HasConstraintName("FK__Like__VideoId__5FB337D6");
        });

        modelBuilder.Entity<ServerLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServerLo__3214EC07FA3AF63A");

            entity.ToTable("ServerLog");

            entity.HasIndex(e => e.Date, "IX_ServerLog_Date");

            entity.HasIndex(e => e.Type, "IX_ServerLog_Type");

            entity.HasIndex(e => new { e.UserId, e.Date }, "IX_ServerLog_UserId_Date");

            entity.HasOne(d => d.User).WithMany(p => p.ServerLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ServerLog__UserI__5CD6CB2B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07736D858B");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "IX_User_Email").IsUnique();

            entity.HasIndex(e => e.Name, "IX_User_Name");

            entity.Property(e => e.Email).HasMaxLength(254);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Video__3214EC07D1F80721");

            entity.ToTable("Video");

            entity.HasIndex(e => e.DateUpload, "IX_Video_DateUpload");

            entity.HasIndex(e => e.IsVerified, "IX_Video_IsVerified");

            entity.HasIndex(e => e.Name, "IX_Video_Name");

            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Poster).HasMaxLength(500);

            entity.HasOne(d => d.Author).WithMany(p => p.Videos)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Video__AuthorId__5441852A");
        });

        modelBuilder.Entity<View>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__View__3214EC07B27338D1");

            entity.ToTable("View");

            entity.HasIndex(e => new { e.VideoId, e.UserId }, "IX_View_VideoId_UserId").IsUnique();

            entity.HasOne(d => d.User).WithMany(p => p.ViewsTable)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__View__UserId__68487DD7");

            entity.HasOne(d => d.Video).WithMany(p => p.ViewsTable)
                .HasForeignKey(d => d.VideoId)
                .HasConstraintName("FK__View__VideoId__6754599E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
