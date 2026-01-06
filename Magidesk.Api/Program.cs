using Magidesk.Application.DependencyInjection;
using Magidesk.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

// F-ENTRY-001 FIX: Startup logging and validation
var startupLogger = new List<string>();
try
{
    startupLogger.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] API Startup - Begin");
    
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    startupLogger.Add("Adding Controllers...");
    builder.Services.AddControllers();
    
    startupLogger.Add("Adding Application services...");
    builder.Services.AddApplication();
    
    startupLogger.Add("Adding Infrastructure services...");
    builder.Services.AddInfrastructure();

    // Health checks
    startupLogger.Add("Adding Health Checks...");
    builder.Services.AddHealthChecks();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    startupLogger.Add("Building application...");
    var app = builder.Build();

    // F-ENTRY-001 FIX: Global Exception Handler
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            var errorResponse = new
            {
                error = "An unexpected error occurred",
                message = exception?.Message ?? "Unknown error",
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.Value
            };

            // Log the exception
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, "Unhandled API exception at {Path}", context.Request.Path);

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        });
    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    // F-ENTRY-001 FIX: Health Check Endpoint
    app.MapHealthChecks("/health");

    // F-ENTRY-001 FIX: Startup Validation
    startupLogger.Add("Validating critical services...");
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        
        // Validate critical services are registered
        try
        {
            var mediator = services.GetRequiredService<MediatR.IMediator>();
            startupLogger.Add("✓ IMediator registered");
        }
        catch (Exception ex)
        {
            startupLogger.Add($"✗ IMediator registration failed: {ex.Message}");
            throw new InvalidOperationException("Critical service IMediator not registered", ex);
        }

        // F-ENTRY-008 FIX (TICKET-005): Database Connection Validation
        startupLogger.Add("Validating database connection...");
        try
        {
            var dbContext = services.GetRequiredService<Magidesk.Infrastructure.Data.ApplicationDbContext>();
            
            // Attempt to connect with retry logic
            int retryCount = 0;
            int maxRetries = 3;
            bool connected = false;
            
            while (retryCount < maxRetries && !connected)
            {
                try
                {
                    // Simple connection test
                    var canConnect = dbContext.Database.CanConnect();
                    if (canConnect)
                    {
                        connected = true;
                        startupLogger.Add("✓ Database connection successful");
                    }
                    else
                    {
                        throw new InvalidOperationException("Database.CanConnect() returned false");
                    }
                }
                catch (Exception dbEx)
                {
                    retryCount++;
                    if (retryCount < maxRetries)
                    {
                        startupLogger.Add($"⚠ Database connection attempt {retryCount} failed: {dbEx.Message}. Retrying in 2 seconds...");
                        System.Threading.Thread.Sleep(2000);
                    }
                    else
                    {
                        startupLogger.Add($"✗ Database connection failed after {maxRetries} attempts: {dbEx.Message}");
                        throw new InvalidOperationException($"Database connection failed after {maxRetries} attempts", dbEx);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            startupLogger.Add($"✗ Database validation failed: {ex.Message}");
            throw new InvalidOperationException("Database validation failed", ex);
        }

        // Validate configuration
        startupLogger.Add("Validating configuration...");
        try
        {
            var config = services.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            
            // Check for required configuration keys
            var connectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string 'DefaultConnection' is missing or empty");
            }
            startupLogger.Add("✓ Configuration validated");
        }
        catch (Exception ex)
        {
            startupLogger.Add($"✗ Configuration validation failed: {ex.Message}");
            throw new InvalidOperationException("Configuration validation failed", ex);
        }
    }

    startupLogger.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] API Startup - Success");
    startupLogger.Add("API is ready to accept requests");
    
    // Log all startup messages
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    foreach (var msg in startupLogger)
    {
        logger.LogInformation(msg);
    }

    app.Run();
}
catch (Exception ex)
{
    // F-ENTRY-001 FIX: Fatal Startup Error Handling
    startupLogger.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] FATAL STARTUP ERROR: {ex.Message}");
    startupLogger.Add($"Stack: {ex.StackTrace}");
    
    // Write to console (always visible)
    Console.Error.WriteLine("═══════════════════════════════════════════════════════");
    Console.Error.WriteLine("MAGIDESK API FATAL STARTUP ERROR");
    Console.Error.WriteLine("═══════════════════════════════════════════════════════");
    foreach (var msg in startupLogger)
    {
        Console.Error.WriteLine(msg);
    }
    Console.Error.WriteLine("═══════════════════════════════════════════════════════");
    
    // Write to file (persistent log)
    try
    {
        var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Magidesk", "Logs");
        Directory.CreateDirectory(logDir);
        var logPath = Path.Combine(logDir, "api_startup_failure.txt");
        File.WriteAllLines(logPath, startupLogger);
        Console.Error.WriteLine($"Startup log written to: {logPath}");
    }
    catch
    {
        Console.Error.WriteLine("Failed to write startup log to file");
    }
    
    // Exit with error code
    Environment.Exit(1);
}
