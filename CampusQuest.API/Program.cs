using CampusQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS to allow requests from mobile app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobileApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key")
    ?? throw new InvalidOperationException("JWT signing key is not configured.");
var jwtIssuer = jwtSection.GetValue<string>("Issuer");
var jwtAudience = jwtSection.GetValue<string>("Audience");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
            ValidIssuer = jwtIssuer,
            ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

    builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapibuilder
builder.Services.AddOpenApi();

// Register DbContext with SQLite Provider
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// 🚀 CRITICAL FIX: Always register the Repository BEFORE the Service!
builder.Services.AddScoped<CampusQuest.Infrastructure.Repositories.UserRepository>();
builder.Services.AddScoped<CampusQuest.Infrastructure.Services.UserService>();

// Port ayarı launchSettings.json > applicationUrl: "http://0.0.0.0:5105" ile gelir.
// Kestrel'i burada tekrar yapılandırmıyoruz — çift bağlanma hatasını önler.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ⛔ HTTPS yönlendirmesi KALDIRILDI.
// Android emülatör/cihaz HTTP ile bağlandığı için UseHttpsRedirection
// portu çakıştırıyor ve bağlantı kopmasına neden oluyordu.
// app.UseHttpsRedirection();  // DEVRE DIŞI

// Ensure database is created and migrations are applied at startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();

        // Make the first run testable without requiring a manual registration.
        // Registration remains available for normal users.
        if (app.Environment.IsDevelopment())
        {
            var users = scope.ServiceProvider.GetRequiredService<CampusQuest.Infrastructure.Services.UserService>();
            // RegisterAsync returns null if user already exists — that is fine.
            await users.RegisterAsync("demo", "demo@campusquest.local", "CampusQuest123!");
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Program");
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
        throw;
    }
}

// Enable CORS
app.UseCors("AllowMobileApp");

// ⛔ The mobile client connects over the development LAN HTTP endpoint.
// Do NOT redirect it to HTTPS: a physical device cannot trust the development cert.

// Enable authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// Health-check endpoint for mobile connectivity testing
app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    utc = DateTime.UtcNow,
    message = "CampusQuest API is running."
}));

app.MapControllers();

app.Run();
