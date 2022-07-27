using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using BlogMvcCore.DomainModel;
using BlogMvcCore.Storage;
using BlogMvcCore.Services;

namespace BlogMvcCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddAppRepository(services);
            AddAppUserService(services);
            services.AddControllersWithViews();
            services.AddDbContext<Storage.AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("UserContext")));
            services.AddSession();
        }

        private void AddAppRepository(IServiceCollection services)
        {
            services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<ICommentRepository, CommentRepository>();
        }

        private void AddAppUserService(IServiceCollection services)
        {
            services.AddTransient<AuthenticationService>();
            services.AddTransient<UserService>();
            services.AddTransient<PostService>();
            services.AddTransient<CommentService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/User/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Authentication}/{action=index}/{id?}");
            });
        }
    }
}
