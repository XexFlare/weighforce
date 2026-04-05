using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeighForce.Data;
using WeighForce.Data.EF;
using WeighForce.Data.Repositories;
using WeighForce.Models;
using WeighForce.Services;
using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            // .AllowAnyOrigin()
            // .AllowAnyMethod()
        });
    }

    public static void ConfigureAuth(this IServiceCollection services)
    {
        services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>()
                .AddInMemoryIdentityResources(GetIdentityResources());

        services.AddAuthentication()
            .AddIdentityServerJwt();

        // TODO: setup token refresh
        services.Configure<JwtBearerOptions>(
                IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
                options =>
                {
                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.TokenValidationParameters.ValidateLifetime = false;
                });
    }
    public static void ConfigureEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddFluentEmail(configuration.GetValue<string>("Email:From"))
            .AddSmtpSender(configuration.GetValue<string>("Email:Server"), configuration.GetValue<int>("Email:Port"), configuration.GetValue<string>("Email:Username"), configuration.GetValue<string>("Email:Password"))
            // .AddSendGridSender(configuration.GetValue<string>("Email:SendGridToken"))
            .AddRazorRenderer();
        services.AddSingleton<EmailService>();
    }
    public static void ConfigureSync(this IServiceCollection services)
    {
        services.AddHttpClient<CrmService>();
        services.AddHttpClient<UserService>();
        services.AddSingleton<SyncService>();
        services.AddHostedService<SyncService>(provider => provider.GetService<SyncService>());
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddHostedService<BackgroundQueueHostedService>();
        services.AddSingleton<SerialPortInterface>();
        services.AddSingleton<SyncStatus>();

    }
    public static void ConfigureDB(this IServiceCollection services, IConfiguration configuration)
    {
        switch (configuration.GetValue<string>("DB:App:Connection", "SQlite"))
        {
            case "SQlite":
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(
                        configuration.GetValue<string>("DB:App:String"))
                );
                break;
            case "MySql":
                services.AddDbContext<ApplicationDbContext>(options =>
                    options
                    .UseMySQL(configuration.GetValue<string>("DB:App:String"))
                );
                break;
            case "SQLServer":
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                    configuration.GetValue<string>("DB:App:String")));
                break;
            default:
                services.AddDbContext<ApplicationDbContext>(
                    options =>
                    options.UseSqlite(
                        configuration.GetValue<string>("DB:App:String"))
                );
                break;
        }
        switch (configuration.GetValue<string>("DB:OnTrack:Connection", "SQlite"))
        {
            case "SQlite":
                services.AddDbContext<OnTractContext>(options =>
                    options.UseSqlite(
                        configuration.GetValue<string>("DB:OnTrack:String"))
                );
                break;
            case "MySql":
                services.AddDbContext<OnTractContext>(options =>
                    options
                    .UseMySQL(configuration.GetValue<string>("DB:OnTrack:String"))
                );
                break;
            case "SQLServer":
                services.AddDbContext<OnTractContext>(options =>
                    options.UseSqlServer(
                    configuration.GetValue<string>("DB:OnTrack:String")));
                break;
            default:
                services.AddDbContext<OnTractContext>(
                    options =>
                    options.UseSqlite(
                        configuration.GetValue<string>("DB:OnTrack:String"))
                );
                break;
        }
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<ICountriesRepository, CountriesRepository>();
        services.AddScoped<IOfficesRepository, OfficesRepository>();
        services.AddScoped<IDispatchesRepository, DispatchesRepository>();
        services.AddScoped<IClientsRepository, ClientsRepository>();
        services.AddScoped<IReportsRepository, ReportsRepository>();
        services.AddScoped<IContractsRepository, ContractsRepository>();
        services.AddScoped<ITIsRepository, TIRepository>();
        services.AddScoped<ITransportersRepository, TransportersRepository>();
        services.AddScoped<IBookingsRepository, BookingsRepository>();
        services.AddScoped<IMetaRepository, MetaRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    public static List<IdentityResource> GetIdentityResources()
    {
        // Claims automatically included in OpenId scope
        var openIdScope = new IdentityResources.OpenId();
        openIdScope.UserClaims.Add(JwtClaimTypes.Locale);

        // Available scopes
        return new List<IdentityResource>
            {
                openIdScope,
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                // new IdentityResource(Constants.RolesScopeType, Constants.RolesScopeType,
                // new List<string> {JwtClaimTypes.Role, Constants.TenantIdClaimType})
                // {
                //     //when false (default), the user can deselect the scope on consent screen
                //     Required = true
                // }
            };
    }
}