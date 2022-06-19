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
            services.AddTransient<IAuthentication, AuthenticationRepository>();
            services.AddTransient<IUser, UserRepository>();
            services.AddTransient<IPost, PostRepository>();
            services.AddTransient<IComment, CommentRepository>();
            services.AddTransient<Authentication>();
            services.AddTransient<UserService>();
            services.AddTransient<PostService>();
            services.AddTransient<CommentService>();
            services.AddControllersWithViews();
            services.AddDbContext<Storage.DbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("UserContext")));
            services.AddSession();
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

            app.UseSession();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Authentication}/{action=index}/{id?}");
            });
        }
    }
}
