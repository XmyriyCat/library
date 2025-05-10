using Library.Api.Extensions;

namespace Library.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.ConfigureMsSqlContext(builder.Configuration);
        builder.Services
            .ConfigureServices()
            .ConfigureRepositories()
            .ConfigureIdentityCore()
            .ConfigureMapster()
            .ConfigureNewtonsoftJson()
            .AddFluentValidators();

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        await app.MigrateDbAsync();
        await app.InitializeDbAsync();
        app.UseValidationMiddleware()
            .UseGlobalExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        app.MapControllers();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        
        await app.RunAsync();
    }
}