using Microsoft.EntityFrameworkCore;

namespace BlogMvcCore.Storage
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
                           : base(options) { Database.EnsureCreated(); }
        public DbSet<UserEntity> BlogUsers { get; set; }
        public DbSet<PostEntity> Posts { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserEntity>(entity =>
            {
                entity.HasIndex(l => l.Login).
                       IsUnique();
            });
            base.OnModelCreating(builder);
        }
    }
}