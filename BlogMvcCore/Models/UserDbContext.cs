using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BlogMvcCore.Models
{
    public class UserDbContext : IdentityDbContext<IdentityUser>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
                           : base(options) { Database.EnsureCreated(); }
        public DbSet<User> BlogUsers { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            User user = new(firstName: "Admin", secondName: "System", login: "admin", password: "12345678");
            user.ID = 1;
            builder.Entity<User>().HasData(user);
        }
    }
}