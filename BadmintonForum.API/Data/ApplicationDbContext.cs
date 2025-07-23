using BadmintonForum.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BadmintonForum.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasMany(c => c.Posts)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Seed initial hierarchical categories
                entity.HasData(
                    // 綜合討論區
                    new Category { Id = 1, Name = "綜合討論區", Description = "羽毛球相關的一般討論", Icon = "💬", DisplayOrder = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

                    // 技術交流區
                    new Category { Id = 2, Name = "技術交流區", Description = "技術分享與教學討論", Icon = "🏸", DisplayOrder = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

                    // 裝備討論區
                    new Category { Id = 3, Name = "裝備討論區", Description = "球拍、球鞋、裝備評測與推薦", Icon = "🎾", DisplayOrder = 3, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

                    // 賽事專區
                    new Category { Id = 4, Name = "賽事專區", Description = "國內外賽事討論與轉播", Icon = "🏆", DisplayOrder = 4, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

                    // 地區球友會
                    new Category { Id = 5, Name = "地區球友會", Description = "各地區球友交流與約球", Icon = "📍", DisplayOrder = 5, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
                );
            });

            // Post configuration
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasOne(p => p.Author)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(p => p.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Replies)
                    .WithOne(r => r.Post)
                    .HasForeignKey(r => r.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Reply configuration
            modelBuilder.Entity<Reply>(entity =>
            {
                entity.HasOne(r => r.Author)
                    .WithMany(u => u.Replies)
                    .HasForeignKey(r => r.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PostLike configuration
            modelBuilder.Entity<PostLike>(entity =>
            {
                // 複合唯一索引，確保每個使用者對每篇文章只能點讚一次
                entity.HasIndex(pl => new { pl.PostId, pl.UserId }).IsUnique();

                entity.HasOne(pl => pl.Post)
                    .WithMany(p => p.PostLikes)
                    .HasForeignKey(pl => pl.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pl => pl.User)
                    .WithMany(u => u.PostLikes)
                    .HasForeignKey(pl => pl.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}