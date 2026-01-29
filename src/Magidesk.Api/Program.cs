using Magidesk.Api.Infrastructure;
using Magidesk.Application.DependencyInjection;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.DependencyInjection;
using Magidesk.Api.Controllers; // Ensure controllers are reachable
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor(); // Required for Http implementations
builder.Services.AddMemoryCache(); // Required for EnhancedCachingService
builder.Services.AddSignalR();
builder.Services.AddSingleton<Magidesk.Application.Interfaces.IKitchenNotificationPublisher, Magidesk.Api.Services.SignalRKitchenNotificationPublisher>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// 1. Wire Layers (assuming extensions exist, otherwise fallback to assembly scanning)
builder.Services.AddInfrastructure();
builder.Services.AddApplication(); 
// Note: If AddApplication is missing, we must scan manually. 
// Assuming it exists based on "Magidesk.Application" check earlier (saw DependencyInjection folder).
// If compile fail, I'll switch to manual MediatR/Scrutor style scanning.

// 2. Wire Context Overrides (Replace Singleton/Desktop versions with HTTP versions)
// Remove any existing WinUI singletons if AddInfrastructure registered them (it didn't, thankfully)
builder.Services.AddScoped<IUserService, HttpUserService>();
builder.Services.AddScoped<ITerminalContext, HttpTerminalContext>();

// 3. Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler(); // Use the Global Handler

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");

// Middleware Order Matters!
app.UseAuthentication(); // If using JWT
app.UseAuthorization();

app.MapControllers();
app.MapHub<Magidesk.Api.Hubs.KitchenHub>("/hubs/kitchen");

app.Run();
