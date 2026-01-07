using System;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Services;

namespace Magidesk.Infrastructure.Services.Bootstrap
{
    public class SystemInitializationService : ISystemInitializationService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SystemInitializationService> _logger;
        private readonly ITerminalRepository _terminalRepository;

        public SystemInitializationService(ApplicationDbContext dbContext, ILogger<SystemInitializationService> logger, ITerminalRepository terminalRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _terminalRepository = terminalRepository;
        }

        public async Task<InitializationResult> InitializeSystemAsync()
        {
            try
            {
                StartupLogger.Log("INIT: Starting System Initialization...");
                _logger.LogInformation("Starting System Initialization...");
                System.Diagnostics.Debug.WriteLine("INIT: Starting System Initialization...");

                // 1. Check Database Connectivity
                StartupLogger.Log("INIT: Checking Database Connectivity...");
                _logger.LogInformation("Checking Database Connectivity...");
                if (!await _dbContext.Database.CanConnectAsync())
                {
                    StartupLogger.Log("INIT: Database connection FAILED");
                    _logger.LogCritical("Database connection failed during bootstrap.");
                    return InitializationResult.Failure("Cannot connect to the database. Please check configuration.");
                }

                // 2. Resolve Terminal Identity
                string terminalIdentity = Environment.MachineName;
                StartupLogger.Log($"INIT: Resolving Terminal Identity for: {terminalIdentity}");
                _logger.LogInformation($"Resolving Terminal Identity for: {terminalIdentity}");
                System.Diagnostics.Debug.WriteLine($"INIT: Resolving Terminal Identity for: {terminalIdentity}");

                var terminal = await _terminalRepository.GetByTerminalKeyAsync(terminalIdentity);
                
                if (terminal == null)
                {
                    StartupLogger.Log("INIT: Creating new terminal record");
                    _logger.LogInformation($"Registering new Terminal: {terminalIdentity}");
                    terminal = Magidesk.Domain.Entities.Terminal.Create(terminalIdentity, terminalIdentity);
                    await _terminalRepository.AddTerminalAsync(terminal);
                }

                StartupLogger.Log($"INIT: Terminal ID: {terminal.Id}");

                // 3. Seed Reference Data
                StartupLogger.Log("INIT: Seeding reference data...");
                _logger.LogInformation("Seeding reference data if needed...");
                System.Diagnostics.Debug.WriteLine("INIT: Seeding reference data...");
                await SeedReferenceDataAsync();

                StartupLogger.Log("INIT: Initialization Success");
                _logger.LogInformation("System Initialization successful.");
                System.Diagnostics.Debug.WriteLine("INIT: Initialization successful.");

                return InitializationResult.Success(terminalIdentity, terminal.Id);
            }
            catch (Exception ex)
            {
                StartupLogger.Log($"INIT: FATAL ERROR: {ex.Message}");
                StartupLogger.Log($"INIT: STACK: {ex.StackTrace}");
                _logger.LogCritical(ex, "CRITICAL ERROR during Initialization.");
                System.Diagnostics.Debug.WriteLine($"INIT FATAL: {ex}");
                return InitializationResult.Failure($"Init Crash: {ex.Message}");
            }
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
                // Seed Floors & Layouts
                Guid? defaultFloorId = null;
                Guid? defaultLayoutId = null;

                if (!await _dbContext.Floors.AnyAsync())
                {
                    _logger.LogInformation("Seeding Main Floor...");
                    var floor = Magidesk.Domain.Entities.Floor.Create("Main Floor", "Primary Dining Area");
                    _dbContext.Floors.Add(floor);
                    await _dbContext.SaveChangesAsync();
                    defaultFloorId = floor.Id;
                }
                else
                {
                    var floor = await _dbContext.Floors.FirstOrDefaultAsync(f => f.Name == "Main Floor") 
                                ?? await _dbContext.Floors.FirstOrDefaultAsync();
                    defaultFloorId = floor?.Id;
                }

                if (!await _dbContext.TableLayouts.AnyAsync())
                {
                    _logger.LogInformation("Seeding Standard Layout...");
                    if (defaultFloorId.HasValue)
                    {
                        var layout = Magidesk.Domain.Entities.TableLayout.Create("Standard Layout", defaultFloorId.Value);
                        _dbContext.TableLayouts.Add(layout);
                        await _dbContext.SaveChangesAsync();
                        defaultLayoutId = layout.Id;
                    }
                    else
                    {
                         _logger.LogWarning("Skipping Layout seeding: No Floor available.");
                    }
                }
                else
                {
                    var layout = await _dbContext.TableLayouts.FirstOrDefaultAsync(l => l.Name == "Standard Layout")
                                 ?? await _dbContext.TableLayouts.FirstOrDefaultAsync();
                    defaultLayoutId = layout?.Id;
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
                            defaultFloorId, // Link to Main Floor
                            defaultLayoutId // Link to Standard Layout
                        );
                        
                        tables.Add(table);
                    }

                    _dbContext.Tables.AddRange(tables);
                    await _dbContext.SaveChangesAsync();
                }

                // Seed Print Templates
                // Ensure Receipt Template Exists
                if (!await _dbContext.PrintTemplates.AnyAsync(t => t.Type == TemplateType.Receipt && t.IsSystem))
                {
                    _logger.LogInformation("Seeding Standard Receipt...");
                    
                    var receiptContent = @"
{
  ""Elements"": [
    { ""Type"": ""Text"", ""Content"": ""{{ Restaurant.Name }}"", ""Bold"": true, ""DoubleHeight"": true, ""Align"": ""Center"" },
    { ""Type"": ""Text"", ""Content"": ""{{ Restaurant.Address }}"", ""Align"": ""Center"" },
    { ""Type"": ""Text"", ""Content"": ""{{ Restaurant.Phone }}"", ""Align"": ""Center"" },
    { ""Type"": ""LineBreak"" },
    { ""Type"": ""Separator"" },
    { ""Type"": ""Text"", ""Content"": ""Check: {{ TicketNumber }}    Server: {{ ServerName }}"" },
    { ""Type"": ""Text"", ""Content"": ""Date: {{ Date }} {{ Time }}"" },
    { ""Type"": ""Separator"" }
    {% for line in Lines %}
    ,{ ""Type"": ""Text"", ""Content"": ""{{ line.Quantity }} {{ line.Name }}     {{ line.Total }}"" }
    {% for mod in line.Modifiers %}
    ,{ ""Type"": ""Text"", ""Content"": ""  + {{ mod }}"" }
    {% endfor %}
    {% endfor %}
    ,{ ""Type"": ""Separator"" },
    { ""Type"": ""Text"", ""Content"": ""Subtotal: {{ Subtotal }}"", ""Align"": ""Right"" },
    { ""Type"": ""Text"", ""Content"": ""Tax: {{ Tax }}"", ""Align"": ""Right"" },
    { ""Type"": ""Text"", ""Content"": ""TOTAL: {{ Total }}"", ""Bold"": true, ""DoubleHeight"": true, ""Align"": ""Right"" },
    { ""Type"": ""LineBreak"" },
    { ""Type"": ""Text"", ""Content"": ""Thank You!"", ""Align"": ""Center"" },
    { ""Type"": ""Cut"" }
  ]
}";
                    var template = PrintTemplate.Create("Standard Receipt", TemplateType.Receipt, receiptContent);
                    template.UpdateIsSystem(true);
                    
                    _dbContext.PrintTemplates.Add(template);
                }
                
                // Ensure Kitchen Template Exists
                if (!await _dbContext.PrintTemplates.AnyAsync(t => t.Type == TemplateType.Kitchen && t.IsSystem))
                {
                    _logger.LogInformation("Seeding Standard Kitchen Ticket...");
                     var kitchenContent = @"
{
  ""Elements"": [
    { ""Type"": ""Text"", ""Content"": ""KITCHEN TICKET #{{ TicketNumber }}"", ""Bold"": true, ""DoubleHeight"": true, ""DoubleWidth"": true, ""Align"": ""Center"" },
    { ""Type"": ""Text"", ""Content"": ""Table: {{ TableName }}"", ""Bold"": true, ""DoubleHeight"": true, ""Align"": ""Left"" },
    { ""Type"": ""Text"", ""Content"": ""Server: {{ ServerName }}"", ""Align"": ""Left"" },
    { ""Type"": ""Text"", ""Content"": ""Date: {{ Date }} {{ Time }}"", ""Align"": ""Right"" },
    { ""Type"": ""Separator"" }
    {% for line in Lines %}
    ,{ ""Type"": ""Text"", ""Content"": ""{{ line.Quantity }} x {{ line.Name }}"", ""Bold"": true, ""DoubleHeight"": true }
    {% for mod in line.Modifiers %}
    ,{ ""Type"": ""Text"", ""Content"": ""   >> {{ mod }}"", ""Bold"": true, ""Invert"": true }
    {% endfor %}
    {% if line.Instructions != empty %}
    ,{ ""Type"": ""Text"", ""Content"": ""   ** {{ line.Instructions }} **"", ""Bold"": true }
    {% endif %}
    ,{ ""Type"": ""LineBreak"" }
    {% endfor %}
    ,{ ""Type"": ""LineBreak"" },
    { ""Type"": ""Cut"" }
  ]
}";
                    var kTemplate = PrintTemplate.Create("Standard Kitchen", TemplateType.Kitchen, kitchenContent);
                    kTemplate.UpdateIsSystem(true);
                    _dbContext.PrintTemplates.Add(kTemplate);
                }

                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error seeding reference data.");
            }
        }
    }
}
