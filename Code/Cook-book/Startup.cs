using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Cook_book.Data;
using Cook_book.Models;


namespace Cook_book
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["FacebookID"];
                facebookOptions.AppSecret = Configuration["FacebookSecret"];
                facebookOptions.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
                {
                    OnRemoteFailure = ctx =>
                    {
                        ctx.Response.Redirect("/Account/Login");
                        ctx.HandleResponse();
                        return Task.FromResult(0);
                    }
                };
            });

            services.AddAuthentication().AddTwitter(twitterOptions =>
            {
                twitterOptions.ConsumerKey = Configuration["TwitterID"];
                twitterOptions.ConsumerSecret = Configuration["TwitterSecret"];
                twitterOptions.Events = new Microsoft.AspNetCore.Authentication.Twitter.TwitterEvents
                {
                    OnRemoteFailure = ctx =>
                    {
                        ctx.Response.Redirect("/Account/Login");
                        ctx.HandleResponse();
                        return Task.FromResult(0);
                    }
                };

            });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["GoogleID"];
                googleOptions.ClientSecret = Configuration["GoogleSecret"];
                googleOptions.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
                {
                    OnRemoteFailure = ctx =>
                    {
                        ctx.Response.Redirect("/Account/Login");
                        ctx.HandleResponse();
                        return Task.FromResult(0);
                    }
                };
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
