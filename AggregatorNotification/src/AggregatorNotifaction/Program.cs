var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Register TelegramService
builder.Services.AddSingleton<INotification>(sp => new TelegramService("TelegramBotToken"));

// Register EmailService with logging
builder.Services.AddSingleton<INotification>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<EmailService>>();
    return new EmailService(logger);
});

builder.Services.AddDaprClient();

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
app.UseRouting();
// Enable CORS
app.UseCors("AllowAll");

app.UseAuthorization();

// UseCloudEvents must be before UseEndpoints
app.UseCloudEvents();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapSubscribeHandler(); // This ensures Dapr can route pub/sub messages
});

app.Run();
