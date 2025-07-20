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

                // Seed some initial categories
                entity.HasData(
                    new Category { Id = 1, Name = "技術討論", Description = "分享和討論羽毛球技術", Icon = "🏸", DisplayOrder = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new Category { Id = 2, Name = "裝備推薦", Description = "球拍、球鞋等裝備討論", Icon = "🎾", DisplayOrder = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new Category { Id = 3, Name = "活動公告", Description = "比賽和活動信息", Icon = "📅", DisplayOrder = 3, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new Category { Id = 4, Name = "球友交流", Description = "尋找球友，組織活動", Icon = "👥", DisplayOrder = 4, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
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
        }
    }
}