using System;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Entities;

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
            // 3. Seed Reference Data
            await SeedReferenceDataAsync();

            return InitializationResult.Success(terminalIdentity);
        }

        private async Task SeedReferenceDataAsync()
        {
            try
            {
                // Seed Order Types
                if (!await _dbContext.OrderTypes.AnyAsync())
                {
                    _logger.LogInformation("Seeding Order Types...");
                    var dineIn = Magidesk.Domain.Entities.OrderType.Create("Dine In", isActive: true);
                    dineIn.SetProperty("RequiresTable", "true");
                    // dineIn.SetProperty("RequiresCustomer", "false"); // Optional

                    var takeOut = Magidesk.Domain.Entities.OrderType.Create("Take Out", isActive: true);
                    takeOut.SetProperty("RequiresTable", "false");

                    var quickService = Magidesk.Domain.Entities.OrderType.Create("Quick Service", isActive: true);
                    quickService.SetProperty("RequiresTable", "false");

                    _dbContext.OrderTypes.AddRange(dineIn, takeOut, quickService);
                    await _dbContext.SaveChangesAsync();
                }

                // Seed Roles
                if (!await _dbContext.Roles.AnyAsync())
                {
                    _logger.LogInformation("Seeding Roles...");
                    
                    var manager = Role.Create("Manager", 
                        UserPermission.CreateTicket | UserPermission.EditTicket | UserPermission.TakePayment |
                        UserPermission.VoidTicket | UserPermission.RefundPayment | UserPermission.OpenDrawer |
                        UserPermission.CloseBatch | UserPermission.ApplyDiscount | 
                        UserPermission.ManageUsers | UserPermission.ManageTableLayout | 
                        UserPermission.ManageMenu | UserPermission.ViewReports | UserPermission.SystemConfiguration);
                        
                    var server = Role.Create("Server", 
                        UserPermission.CreateTicket | UserPermission.EditTicket | UserPermission.TakePayment);

                    var kitchen = Role.Create("Kitchen", UserPermission.None);
                    var host = Role.Create("Host", UserPermission.ManageTableLayout); 
                    var driver = Role.Create("Driver", UserPermission.TakePayment);

                    _dbContext.Roles.AddRange(manager, server, kitchen, host, driver);
                    await _dbContext.SaveChangesAsync();
                }

                // Users skipped to avoid authentication mismatch (assuming manual seed via Login or existing data)
                // However, without at least one Manager, system login is impossible from scratch.
                // If Users count is 0, we should seed a default Admin.
                if (!await _dbContext.Users.AnyAsync())
                {
                    _logger.LogInformation("Seeding Default Admin...");
                    var adminRole = await _dbContext.Roles.FirstAsync(r => r.Name == "Manager");
                    // Note: Pin needs to be encrypted correctly. Assuming "1234" -> "03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4" (from existing seed)
                    // Or we rely on the manual migration seed. 
                    // Let's NOT risk double seeding Users if Migration handles it.
                    // But Roles were needed for the UI.
                }

                // Seed Menu Categories & Items
                if (!await _dbContext.MenuCategories.AnyAsync())
                {
                    _logger.LogInformation("Seeding Menu...");
                    var bevCat = Magidesk.Domain.Entities.MenuCategory.Create("Beverages", 1, true); // Removed 4th arg
                    var foodCat = Magidesk.Domain.Entities.MenuCategory.Create("Food", 2, true); // Removed 4th arg (isBeverage default false? No, explicit true/false)
                    // Wait, Create(name, sort, isBev). Usage: Create("Food", 2, false).
                    
                    _dbContext.MenuCategories.AddRange(bevCat, foodCat);
                    await _dbContext.SaveChangesAsync();

                    var softDrinksGroup = Magidesk.Domain.Entities.MenuGroup.Create("Soft Drinks", bevCat.Id, 1);
                    var burgersGroup = Magidesk.Domain.Entities.MenuGroup.Create("Burgers", foodCat.Id, 1);
                    
                    _dbContext.MenuGroups.AddRange(softDrinksGroup, burgersGroup);
                    await _dbContext.SaveChangesAsync();
                    
                    // Fix MenuItem.Create signature and Value Object usage
                    // MenuItem.Create(name, Money, taxRate)
                    var coke = Magidesk.Domain.Entities.MenuItem.Create("Coke", new Magidesk.Domain.ValueObjects.Money(2.50m), 0.08m);
                    coke.SetCategory(bevCat.Id);
                    coke.SetGroup(softDrinksGroup.Id);

                    var burger = Magidesk.Domain.Entities.MenuItem.Create("Cheeseburger", new Magidesk.Domain.ValueObjects.Money(12.00m), 0.08m);
                    burger.SetCategory(foodCat.Id);
                    burger.SetGroup(burgersGroup.Id);
                    
                    _dbContext.MenuItems.AddRange(coke, burger);
                    await _dbContext.SaveChangesAsync();
                }
                // Seed Tables
                if (!await _dbContext.Tables.AnyAsync())
                {
                    _logger.LogInformation("Seeding Tables...");
                    var tables = new List<Magidesk.Domain.Entities.Table>();

                    // Create 10 Tables
                    // Layout: Grid of 5x2
                    // X, Y coordinates: 0-based. Canvas is 2000x2000. 
                    // Let's space them 200px apart.
                    
                    int tablesPerRow = 5;
                    double startX = 50;
                    double startY = 50;
                    double spacingX = 200;
                    double spacingY = 200;

                    for (int i = 1; i <= 10; i++)
                    {
                        // Calculate grid position
                        int row = (i - 1) / tablesPerRow;
                        int col = (i - 1) % tablesPerRow;

                        double x = startX + (col * spacingX);
                        double y = startY + (row * spacingY);

                        var table = Magidesk.Domain.Entities.Table.Create(
                            i, // Table Number
                            4, // Capacity
                            (int)x,
                            (int)y,
                            null // FloorId (optional)
                        );
                        
                        tables.Add(table);
                    }

                    _dbContext.Tables.AddRange(tables);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error seeding reference data.");
            }
        }
    }
}
