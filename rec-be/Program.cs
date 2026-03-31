using Microsoft.EntityFrameworkCore;
using rec_be;
using rec_be.Data;
using rec_be.Interfaces.Factory;
using rec_be.Interfaces.Repository;
using rec_be.Interfaces.Services;
using rec_be.Repository;
using rec_be.Room_FactoryStrategy.Factory;
using rec_be.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// PostgreSQL
builder.Services.AddDbContext<RACPostgreSQLDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

// ── Repositories ───────────────────────────────────────────────────
builder.Services.AddScoped<IRoomRepository,         PostgreSQLRoomRepository>();
builder.Services.AddScoped<IBookingRepository,      PostgreSQLBookingRepository>();
builder.Services.AddScoped<IGuestRepository,        PostgreSQLGuestRepository>();
builder.Services.AddScoped<IConfigRepository,       PostgreSQLConfigRepository>();
builder.Services.AddScoped<ILateCheckOutRepository, PostgreSQLLateCheckOutRepository>();

// ── Factory (no runtime deps → safe to DI-register) ───────────────
builder.Services.AddScoped<IRoomStrategyFactory, RoomStrategyFactory>();

// ── Services ───────────────────────────────────────────────────────
builder.Services.AddScoped<IBookingService,      BookingService>();
builder.Services.AddScoped<IRoomService,         RoomService>();
builder.Services.AddScoped<IGuestService,        GuestService>();
builder.Services.AddScoped<ILateCheckOutService, LateCheckOutService>();

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