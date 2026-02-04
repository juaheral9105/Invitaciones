using Microsoft.EntityFrameworkCore;
using InvitacionesAPI.Data;
using InvitacionesAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Use PostgreSQL database
Console.WriteLine($"=== CONNECTION STRING DEBUG ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

// Try multiple ways to get connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"GetConnectionString result is null: {connectionString == null}");

// Try reading directly from environment variable
var envConnString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
Console.WriteLine($"Environment variable 'ConnectionStrings__DefaultConnection' is null: {envConnString == null}");

// Try Railway's DATABASE_PUBLIC_URL or DATABASE_URL
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL")
                  ?? Environment.GetEnvironmentVariable("DATABASE_URL");
Console.WriteLine($"DATABASE_URL/DATABASE_PUBLIC_URL is null: {databaseUrl == null}");
Console.WriteLine($"DATABASE_URL value (first 30 chars): {(databaseUrl != null ? databaseUrl.Substring(0, Math.Min(30, databaseUrl.Length)) : "null")}");

if (!string.IsNullOrEmpty(databaseUrl))
{
    Console.WriteLine($"DATABASE_URL starts with 'postgres://': {databaseUrl.StartsWith("postgres://")}");
    Console.WriteLine($"DATABASE_URL starts with 'postgresql://': {databaseUrl.StartsWith("postgresql://")}");

    if (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://"))
    {
        try
        {
            // Convert PostgreSQL URL format to .NET connection string format
            // Format: postgres://user:password@host:port/database or postgresql://...
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');
            connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
            Console.WriteLine("Successfully converted DATABASE_URL to connection string");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting DATABASE_URL: {ex.Message}");
        }
    }
}
else if (!string.IsNullOrEmpty(envConnString))
{
    connectionString = envConnString;
    Console.WriteLine("Using ConnectionStrings__DefaultConnection");
}

Console.WriteLine($"Final connection string is null: {connectionString == null}");
Console.WriteLine($"Final connection string is empty: {string.IsNullOrEmpty(connectionString)}");
if (!string.IsNullOrEmpty(connectionString))
{
    // Print without password for security
    var sanitized = connectionString.Contains("Password=")
        ? connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=****"
        : connectionString;
    Console.WriteLine($"Connection string: {sanitized}");
}
else
{
    Console.WriteLine("WARNING: No connection string found!");
}
Console.WriteLine($"==============================");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is not configured. Please set DATABASE_URL, DATABASE_PUBLIC_URL, or ConnectionStrings__DefaultConnection environment variable.");
}

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
Console.WriteLine("=== STARTING DATABASE MIGRATIONS ===");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine("Getting pending migrations...");
        var pendingMigrations = context.Database.GetPendingMigrations().ToList();
        Console.WriteLine($"Pending migrations count: {pendingMigrations.Count}");
        foreach (var migration in pendingMigrations)
        {
            Console.WriteLine($"  - {migration}");
        }

        Console.WriteLine("Applying migrations...");
        context.Database.Migrate();
        Console.WriteLine("Migrations applied successfully!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        Console.WriteLine($"Migration error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}
Console.WriteLine("=== DATABASE MIGRATIONS COMPLETED ===");

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
app.MapGet("/", () => "API is running! v2");
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();
