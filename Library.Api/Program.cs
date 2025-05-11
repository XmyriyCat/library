using Library.Api.Extensions;
using Library.Api.Variables;

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
            .AddFluentValidators()
            .ConfigureRedisCaching(builder.Configuration)
            .ConfigureCors();

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        //builder.WebHost.UseUrls("http://+:80");
        
        var app = builder.Build();

        await app.MigrateDbAsync();
        await app.InitializeDbAsync();
        app.UseGlobalExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        app.UseCors(CorsValues.PolicyName);
        app.MapControllers();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        
        await app.RunAsync();
    }
}