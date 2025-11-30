using Microsoft.EntityFrameworkCore;
using SofijaFesis_5DanaUOblacima.Data;
using SofijaFesis_5DanaUOblacima.Services;
using SofijaFesis_5DanaUOblacima.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("CanteenReservationDb"));

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICanteenService, CanteenService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ReservationValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Canteen Reservation System API",
        Version = "v1",
        Description = "API for managing canteen reservations",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Sofija Fesis",
            Email = "sofija@example.com"
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// In Docker, bind to all interfaces; locally, use localhost
var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://localhost:8080";
builder.WebHost.UseUrls(urls);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Canteen Reservation API V1");
    });
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();