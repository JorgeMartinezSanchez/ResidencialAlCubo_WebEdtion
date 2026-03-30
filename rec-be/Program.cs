using Microsoft.EntityFrameworkCore;
using rec_be.Data;
using rec_be.Interfaces.Repository;
using rec_be.Interfaces.Services;
using rec_be.Repository;   // tus implementaciones concretas
using rec_be.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// PostgreSQL
builder.Services.AddDbContext<RACPostgreSQLDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IRoomRepository,    PostgreSQLRoomRepository>();
builder.Services.AddScoped<IBookingRepository, PostgreSQLBookingRepository>();
builder.Services.AddScoped<IGuestRepository,   GuestRepository>();
builder.Services.AddScoped<IConfigRepository,  ConfigRepository>();
 
// ── Servicios ─────────────────────────────────────────────────────
builder.Services.AddScoped<IBookingService, BookingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Residencial Al Cubo Web API";
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();