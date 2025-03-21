using System.Data;
using Npgsql;
using Dapper;
using UnivRoomAPI.Interface;
using UnivRoomAPI.Services;
using UnivRoomAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Get PostgreSQL connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register database connection
builder.Services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(connectionString));

// Register repositories and services
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

// Register controllers
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()  // Allows all origins
              .AllowAnyMethod()  // Allows all HTTP methods (GET, POST, etc.)
              .AllowAnyHeader(); // Allows all headers
    });
});

var app = builder.Build();

// Middleware configuration
app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins"); // Enable the CORS policy

app.UseAuthorization();
app.MapControllers();

app.Run();
