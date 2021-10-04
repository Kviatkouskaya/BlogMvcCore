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
    }
}