using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BlogMvcCore.Storage
{
    public class DbContext : IdentityDbContext<IdentityUser>
    {
        public DbContext(DbContextOptions<DbContext> options)
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