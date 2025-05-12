using System.Text;
using FluentValidation;
using Library.Api.Middlewares;
using Library.Api.Variables;
using Library.Application;
using Library.Application.Infrastructure.Mapster;
using Library.Application.Services.Contracts;
using Library.Application.Services.Implementations;
using Library.Data;
using Library.Data.DbStartup;
using Library.Data.Models;
using Library.Data.Repositories.Contracts;
using Library.Data.Repositories.Implementations;
using Library.Data.UnitOfWork;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Library.Api.Extensions;

public static class ServiceExtension
{
    public static void ConfigureMsSqlContext(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config["ConnectionStrings:SqlServerConnection"];

        services.AddDbContext<IdentityDataContext>(options => options.UseSqlServer(connectionString));
    }

    public static async Task<IApplicationBuilder> MigrateDbAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetService<IdentityDataContext>();

        await context!.Database.MigrateAsync();

        return app;
    }

    public static async Task<IApplicationBuilder> InitializeDbAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetService<IdentityDataContext>()!;
        var config = services.GetService<IConfiguration>()!;

        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        var initializerDb = new DatabaseInitializer();
        await initializerDb.InitializeAsync(context, config, userManager, roleManager);

        return app;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserBookService, UserBookService>();

        return services;
    }

    public static IServiceCollection ConfigureIdentityCore(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;

                // Added a space char
                options.User.AllowedUserNameCharacters += " ";
            })
            .AddEntityFrameworkStores<IdentityDataContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserBookRepository, UserBookRepository>();
        
        return services;
    }

    public static IServiceCollection ConfigureMapster(this IServiceCollection services)
    {
        services.AddMapster();
        MappingProfile.Configure();

        return services;
    }

    public static IServiceCollection ConfigureNewtonsoftJson(this IServiceCollection services)
    {
        services.AddControllersWithViews()
            .AddNewtonsoftJson(opt =>
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

        return services;
    }

    public static IServiceCollection AddFluentValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);

        return services;
    }

    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandler>();

        return app;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsValues.PolicyName, policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    public static IServiceCollection ConfigureRedisCaching(this IServiceCollection services, IConfiguration config)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config["Cache:Redis"];
            options.InstanceName = "LibraryCache_";
        });

        return services;
    }

    public static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = config["Jwt:Issuer"]!,
                ValidAudience = config["Jwt:Audience"]!,
                ValidateIssuer = true,
                ValidateAudience = true,
            };
        });

        return services;
    }

    public static IServiceCollection ConfigureJwtAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(x =>
        {
            x.AddPolicy(AuthConstants.AdminPolicyName,
                p => p.RequireClaim(
                    AuthConstants.RoleClaimName, AuthConstants.AdminClaimValue));

            x.AddPolicy(AuthConstants.ManagerPolicyName,
                p => p.RequireAssertion(c =>
                    c.User.HasClaim(m => m is { Type: AuthConstants.RoleClaimName, Value: AuthConstants.AdminClaimValue }) ||
                    c.User.HasClaim(m => m is { Type: AuthConstants.RoleClaimName, Value: AuthConstants.ManagerClaimValue })));
        });

        return services;
    }
}