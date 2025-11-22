// T022-T023: Configure dependency injection, services, and CORS
using FluentValidation;
using FluentValidation.AspNetCore;
using TeamGoalTracker.Api.Data;
using TeamGoalTracker.Api.Data.Repositories;
using TeamGoalTracker.Api.Middleware;
using TeamGoalTracker.Api.Services;
using TeamGoalTracker.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateGoalRequestValidator>();

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost,1433;Database=TeamGoalTracker;User=sa;Password=YourStrong!Password;TrustServerCertificate=True";
builder.Services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(connectionString));

// Register repositories
builder.Services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<IMoodRepository, MoodRepository>();

// Register services
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IMoodService, MoodService>();

// CORS policy for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handler
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
