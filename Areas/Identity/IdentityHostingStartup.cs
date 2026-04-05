using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeighForce.Data;
using WeighForce.Models;

[assembly: HostingStartup(typeof(WeighForce.Areas.Identity.IdentityHostingStartup))]
namespace WeighForce.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                switch (context.Configuration.GetValue<string>("DB:ID:Connection", "SQlite"))
                {
                    case "SQlite":
                        services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlite(
                                context.Configuration.GetValue<string>("DB:ID:String"))
                        );
                        break;
                    case "MySql":
                        services.AddDbContext<ApplicationDbContext>(options =>
                            options
                            .UseMySQL(context.Configuration.GetValue<string>("DB:ID:String"))
                        );
                        break;
                    case "SQLServer":
                        services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(
                            context.Configuration.GetValue<string>("DB:ID:String")));
                        break;
                    default:
                        services.AddDbContext<ApplicationDbContext>(
                            options =>
                            options.UseSqlite(
                                context.Configuration.GetValue<string>("DB:ID:String"))
                        );
                        break;
                }

                services.AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            });
        }
    }
}