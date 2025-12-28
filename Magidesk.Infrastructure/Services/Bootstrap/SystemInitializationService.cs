using System;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Magidesk.Infrastructure.Services.Bootstrap
{
    public class SystemInitializationService : ISystemInitializationService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SystemInitializationService> _logger;

        public SystemInitializationService(ApplicationDbContext dbContext, ILogger<SystemInitializationService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<InitializationResult> InitializeSystemAsync()
        {
            _logger.LogInformation("Starting System Initialization...");

            // 1. Check Database Connectivity
            try
            {
                if (!await _dbContext.Database.CanConnectAsync())
                {
                    _logger.LogCritical("Database connection failed during bootstrap.");
                    return InitializationResult.Failure("Cannot connect to the database. Please check configuration.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Exception during database connection check.");
                return InitializationResult.Failure($"Database Error: {ex.Message}");
            }

            // 2. Resolve Terminal Identity
            // GAP: Terminal Entity does not exist in Domain yet.
            // Strategy: Use Environment.MachineName as the persistent identity for now.
            string terminalIdentity = Environment.MachineName;
            _logger.LogInformation($"Resolved Terminal Identity: {terminalIdentity}");

            // 3. Load Critical Configuration (Stub for now)
            // TODO: Load Tax & Currency from DB

            return InitializationResult.Success(terminalIdentity);
        }
    }
}
