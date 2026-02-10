using Microsoft.EntityFrameworkCore;
using InvitacionesAPI.Data;
using InvitacionesAPI.Services;
using System.Collections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Use PostgreSQL database - Get connection string from environment variables or config
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Try Railway's DATABASE_PUBLIC_URL or DATABASE_URL if not found in config
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL")
                  ?? Environment.GetEnvironmentVariable("DATABASE_URL");

 logger.LogInformation("=== ENV VARS ===");
foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
{
    if (env.Key.ToString()!.StartsWith("Email__"))
    {
        logger.LogInformation($"{env.Key} = {env.Value}");
    }
}

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
        var logger = services.GetRequiredService<ILogger<Program>>();

        // Try to create the database if it doesn't exist
        context.Database.EnsureCreated();

        // Execute raw SQL to create tables if they don't exist
        try
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS ""StoredFiles"" (
                    ""Id"" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
                    ""FileName"" text NOT NULL,
                    ""ContentType"" text NOT NULL,
                    ""Data"" bytea NOT NULL,
                    ""Size"" bigint NOT NULL,
                    ""UploadedAt"" timestamp with time zone NOT NULL,
                    ""FileType"" text NOT NULL
                );
                CREATE INDEX IF NOT EXISTS ""IX_StoredFiles_UploadedAt"" ON ""StoredFiles"" (""UploadedAt"");
                CREATE INDEX IF NOT EXISTS ""IX_StoredFiles_FileType"" ON ""StoredFiles"" (""FileType"");

                CREATE TABLE IF NOT EXISTS ""GuestList"" (
                    ""Id"" serial PRIMARY KEY,
                    ""InvitationId"" uuid NOT NULL,
                    ""Name"" text NOT NULL,
                    ""Phone"" text NOT NULL,
                    ""NumberOfGuests"" integer NOT NULL,
                    ""GuestNames"" text,
                    ""EventHasAccommodation"" boolean NOT NULL DEFAULT false,
                    ""AccommodationAllowed"" boolean NOT NULL DEFAULT false,
                    ""CreatedAt"" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ""UpdatedAt"" timestamp with time zone
                );
                CREATE INDEX IF NOT EXISTS ""IX_GuestList_Phone"" ON ""GuestList"" (""Phone"");
                CREATE INDEX IF NOT EXISTS ""IX_GuestList_InvitationId"" ON ""GuestList"" (""InvitationId"");
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_GuestList_InvitationId_Phone"" ON ""GuestList"" (""InvitationId"", ""Phone"");
            ";
            context.Database.ExecuteSqlRaw(sql);
            logger.LogInformation("Database tables created or already exist.");

            // Add cover page columns to Invitations table if they don't exist
            try
            {
                var coverPageSql = @"
                    DO $$
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Invitations' AND column_name='CoverEnabled') THEN
                            ALTER TABLE ""Invitations"" ADD COLUMN ""CoverEnabled"" boolean NOT NULL DEFAULT true;
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Invitations' AND column_name='CoverName') THEN
                            ALTER TABLE ""Invitations"" ADD COLUMN ""CoverName"" character varying(200) NOT NULL DEFAULT 'Nombre de la Homenajeada';
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Invitations' AND column_name='CoverButtonText') THEN
                            ALTER TABLE ""Invitations"" ADD COLUMN ""CoverButtonText"" character varying(100) NOT NULL DEFAULT 'Ver Invitación';
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Invitations' AND column_name='CoverBackgroundColor') THEN
                            ALTER TABLE ""Invitations"" ADD COLUMN ""CoverBackgroundColor"" character varying(50) NOT NULL DEFAULT '#f3e5f5';
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Invitations' AND column_name='CoverBackgroundImage') THEN
                            ALTER TABLE ""Invitations"" ADD COLUMN ""CoverBackgroundImage"" character varying(500);
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Invitations' AND column_name='CoverTextColor') THEN
                            ALTER TABLE ""Invitations"" ADD COLUMN ""CoverTextColor"" character varying(50) NOT NULL DEFAULT '#ffffff';
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Invitations' AND column_name='CoverButtonColor') THEN
                            ALTER TABLE ""Invitations"" ADD COLUMN ""CoverButtonColor"" character varying(50) NOT NULL DEFAULT '#ffffff';
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Invitations' AND column_name='CoverButtonTextColor') THEN
                            ALTER TABLE ""Invitations"" ADD COLUMN ""CoverButtonTextColor"" character varying(50) NOT NULL DEFAULT '#000000';
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Invitations' AND column_name='CoverFontFamily') THEN
                            ALTER TABLE ""Invitations"" ADD COLUMN ""CoverFontFamily"" character varying(100) NOT NULL DEFAULT 'Playfair Display';
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Invitations' AND column_name='CoverNameFontSize') THEN
                            ALTER TABLE ""Invitations"" ADD COLUMN ""CoverNameFontSize"" character varying(50) NOT NULL DEFAULT '6rem';
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Invitations' AND column_name='CoverButtonFontSize') THEN
                            ALTER TABLE ""Invitations"" ADD COLUMN ""CoverButtonFontSize"" character varying(50) NOT NULL DEFAULT '1.125rem';
                        END IF;
                    END $$;
                ";
                context.Database.ExecuteSqlRaw(coverPageSql);
                logger.LogInformation("Cover page columns added or already exist.");
            }
            catch (Exception ex3)
            {
                logger.LogError(ex3, "Error adding cover page columns: {Message}", ex3.Message);
            }
        }
        catch (Exception ex2)
        {
            logger.LogError(ex2, "Error creating database tables: {Message}", ex2.Message);
        }
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
