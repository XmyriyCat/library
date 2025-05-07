using Library.Data;

namespace Library.Api.Extensions;

public static class ServiceExtension
{
    // public static void ConfigureMsSqlContext(this IServiceCollection services, IConfiguration config)
    // {
    //     var connectionString = config["ConnectionStrings:SqlServerConnection"];
    //
    //     services.AddDbContext<IdentityDataContext>(options => options.UseSqlServer(connectionString));
    // }
    //
    // public static async Task MigrateDbAsync(this IApplicationBuilder app)
    // {
    //     using var scope = app.ApplicationServices.CreateScope();
    //     var services = scope.ServiceProvider;
    //
    //     var context = services.GetService<IdentityDataContext>();
    //
    //     await context.Database.MigrateAsync();
    // }
}