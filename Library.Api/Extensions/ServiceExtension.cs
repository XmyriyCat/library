using FluentValidation;
using Library.Api.Middlewares;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        var initializerDb = new DatabaseInitializer();
        await initializerDb.InitializeAsync(context, userManager, roleManager);

        return app;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBookService, BookService>();

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

        return services;
    }

    public static IServiceCollection ConfigureMapster(this IServiceCollection services)
    {
        // Register Mapster
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

    public static IApplicationBuilder UseValidationMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ValidationMiddleware>();

        return app;
    }
}