using Microsoft.EntityFrameworkCore;
using InvitacionesAPI.Data;
using InvitacionesAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Use PostgreSQL database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"=== CONNECTION STRING DEBUG ===");
Console.WriteLine($"Connection string is null: {connectionString == null}");
Console.WriteLine($"Connection string is empty: {string.IsNullOrEmpty(connectionString)}");
if (!string.IsNullOrEmpty(connectionString))
{
    // Print without password for security
    var sanitized = connectionString.Contains("Password=")
        ? connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=****"
        : connectionString;
    Console.WriteLine($"Connection string: {sanitized}");
}
Console.WriteLine($"==============================");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ExcelParserService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Add CORS - Allow localhost and Vercel domains
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:5175",
                "http://localhost:3000",
                "https://localhost:5173",
                "https://localhost:5174",
                "https://localhost:5175",
                "https://localhost:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });

    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost") ||
                origin.StartsWith("https://localhost") ||
                origin.Contains(".vercel.app"))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Run database migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Create wwwroot/uploads directories if they don't exist
var uploadsPath = Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "uploads");
Directory.CreateDirectory(Path.Combine(uploadsPath, "images"));
Directory.CreateDirectory(Path.Combine(uploadsPath, "music"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use AllowAll policy to support Vercel deployments
app.UseCors("AllowAll");

// Serve static files from wwwroot
app.UseStaticFiles();

// Disable HTTPS redirection in development
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/", () => "API is running!");
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();
