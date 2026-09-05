using System.Text.Json.Serialization;
using InsuranceClaims;
using InsuranceClaims.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MySQL Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=claims.db")
);

builder.Services.AddAutoMapper(
    config =>
    {
        if (!builder.Environment.IsDevelopment())
        {
            var licenseKey = builder.Configuration["AutoMapper:LicenseKey"];
            if (!string.IsNullOrWhiteSpace(licenseKey))
                config.LicenseKey = licenseKey;
        }
    },
    typeof(ProjectModule).Assembly
);

builder.Services.AddProjectModule();

// Convert all URLs to lowercase
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Configure CORS (for frontend later)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Insurance Claims API");
        options.RoutePrefix = "docs";
    });
}

app.UseCors();
app.MapControllers();

app.Run();