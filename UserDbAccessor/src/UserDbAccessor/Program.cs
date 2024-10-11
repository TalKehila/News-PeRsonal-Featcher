public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddDaprClient();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });

        builder.Services.AddControllers().AddDapr();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Enable CORS
        app.UseCors("AllowAllOrigins");

        // Ensure routing is added before endpoints
        app.UseRouting();

        // UseCloudEvents must be before UseEndpoints
        app.UseCloudEvents();

        // Authorization middleware should be added before endpoints
        app.UseAuthorization();

        // Ensure app.MapControllers() is called within the UseEndpoints method
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapSubscribeHandler(); // This ensures Dapr can route pub/sub messages
        });

        // Apply any pending migrations
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            dbContext.Database.Migrate();
        }

        app.Run();
    }
}
