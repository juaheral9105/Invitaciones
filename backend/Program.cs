using Microsoft.EntityFrameworkCore;
using InvitacionesAPI.Data;
using InvitacionesAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Use PostgreSQL database - Get connection string from environment variables or config
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Try Railway's DATABASE_PUBLIC_URL or DATABASE_URL if not found in config
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL")
                  ?? Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl) && (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://")))
{
    // Convert PostgreSQL URL format to .NET connection string format
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else if (string.IsNullOrEmpty(connectionString))
{
    // Try ConnectionStrings__DefaultConnection environment variable
    connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
}

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

// Ensure database and tables exist on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while ensuring the database exists.");
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
app.MapGet("/", () => "API is running! v3 - Database storage enabled");
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow, version = "3.0" });

app.Run();
