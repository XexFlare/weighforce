using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace WeighForce
{
    public class Startup
    {
        IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureDB(Configuration);
            services.ConfigureAuth();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddSignalR();
            services.ConfigureSync();
            services.ConfigureEmail(Configuration);
            services.ConfigureCors();
            if (_env.IsProduction() && !Configuration.GetValue<bool>("LocalDeployment"))
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                  {
                      options.ForwardedHeaders =
                            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                  });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                if (Configuration.GetValue<bool>("HTTPSRedirect"))
                    app.UseHttpsRedirection();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                if (!Configuration.GetValue<bool>("LocalDeployment"))
                    app.UseForwardedHeaders();
                if (Configuration.GetValue<bool>("HTTPSRedirect"))
                    app.UseHttpsRedirection();
            }
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<WeightHub>("/weighthub");
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp/dist";
            });
        }
    }
}
