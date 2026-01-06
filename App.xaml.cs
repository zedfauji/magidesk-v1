using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using MediatR;
using Magidesk.Application.DependencyInjection;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.DTOs;
using Magidesk.Infrastructure.DependencyInjection;
using Magidesk.Infrastructure.Repositories;
using Magidesk.Infrastructure.Services;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Queries.Reports;
using Magidesk.Application.Services.Reports;

namespace Magidesk.Presentation;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Microsoft.UI.Xaml.Application
{
    private Window? _window;

    public static Window? MainWindowInstance { get; private set; }

    public static IHost Host { get; private set; } = null!;

    public static IServiceProvider Services => Host.Services;

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

    public App()
    {
        // SYS-001: FATAL STARTUP BARRIER
        StartupLogger.Log("App Constructor - Start");
        // T-001: Global Background Exception Handlers
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        this.UnhandledException += App_UnhandledException;
        
        StartupLogger.Log("App - InitializeComponent Start");
        try {
            InitializeComponent();
            StartupLogger.Log("App - InitializeComponent Success");
        } catch (Exception ex) {
            HandleFatalStartupError("InitializeComponent Failed", ex);
            throw;
        }

        StartupLogger.Log("App - Host Building Start");
        try {
            Host = Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    StartupLogger.Log("App - ConfigureServices Start");
                    // Application + Infrastructure composition root
                    services.AddApplication();
                    StartupLogger.Log("App - AddApplication Success");
                    services.AddInfrastructure();
                    StartupLogger.Log("App - AddInfrastructure Success");

                    // UI services
                    services.AddSingleton<NavigationService>();
                    services.AddSingleton<IDefaultViewRoutingService, DefaultViewRoutingService>();
                    services.AddSingleton<IOrderEntryDialogService, OrderEntryDialogService>();
                    services.AddSingleton<ISwitchboardDialogService, SwitchboardDialogService>();
                    services.AddSingleton<IDialogService, WindowsDialogService>();
                    services.AddSingleton<IUserService, UserService>();
                    services.AddSingleton<Magidesk.Application.Interfaces.ITerminalContext, TerminalContext>();
            // Printing
            services.AddSingleton<Magidesk.Application.Interfaces.IPrintingService, Infrastructure.Services.PrintingService>();
            services.AddSingleton<Magidesk.Application.Interfaces.IRawPrintService, Infrastructure.Printing.WindowsPrintingService>();
            services.AddScoped<Magidesk.Application.Interfaces.IKitchenPrintService, Infrastructure.Printing.KitchenPrintService>(); // Real Implementation
            services.AddSingleton<Magidesk.Application.Interfaces.ICashDrawerService, Infrastructure.Services.CashDrawerService>();
            services.AddTransient<Magidesk.Application.Interfaces.ICommandHandler<Magidesk.Application.Commands.OpenCashDrawerCommand>, Magidesk.Application.Commands.OpenCashDrawerCommandHandler>();
            services.AddScoped<Magidesk.Application.Interfaces.IReceiptPrintService, Infrastructure.Printing.ReceiptPrintService>(); // Real Implementation

                    // ViewModels
                    StartupLogger.Log("App - Registering ViewModels...");
                    services.AddTransient<Magidesk.Presentation.ViewModels.OrderEntryViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.BackOfficeViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.MenuEditorViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.ModifierEditorViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.InventoryViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.CashSessionViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.TicketViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.PaymentViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.DiscountTaxViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.PrintViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.TicketManagementViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.DrawerPullReportViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.SalesReportsViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.UserManagementViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.RoleManagementViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.SettingsViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.OrderTypeExplorerViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.ShiftExplorerViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.SwitchboardViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.OrderTypeSelectionViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.ModifierSelectionViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.SplitTicketViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.TableMapViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.TableExplorerViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.VendorManagementViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.PurchaseOrderViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.SettleViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.SystemConfigViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.KitchenDisplayViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.LoginViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.ShiftStartViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.CashDropManagementViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.VoidTicketViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.OpenTicketsListViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.PaymentProcessWaitViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.SwipeCardViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.AuthorizationCodeViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.AuthorizationCaptureBatchViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.GuestCountViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.ManagerFunctionsViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.GroupSettleTicketSelectionViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.GroupSettleTicketViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.Dialogs.TableSelectionViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.Dialogs.NotesDialogViewModel>();
                    services.AddTransient<TableDesignerViewModel>();
                    services.AddTransient<FloorManagementViewModel>();
                    services.AddTransient<ExportImportManagementViewModel>();
                    services.AddTransient<ServerSectionManagementViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.PrintTemplatesViewModel>();
                    services.AddTransient<Magidesk.Presentation.ViewModels.TemplateEditorViewModel>();

                    // Query handlers
                    services.AddScoped<IQueryHandler<GetServerProductivityReportQuery, ServerProductivityReportDto>, GetServerProductivityReportQueryHandler>();
                    services.AddScoped<IQueryHandler<GetHourlyLaborReportQuery, HourlyLaborReportDto>, GetHourlyLaborReportQueryHandler>();
                    services.AddScoped<IQueryHandler<GetTipReportQuery, TipReportDto>, GetTipReportQueryHandler>();

                    // Command handlers
                    services.AddTransient<ICommandHandler<GroupSettleCommand, GroupSettleResult>, GroupSettleCommandHandler>();
                    services.AddTransient<ICommandHandler<SplitBySeatCommand, SplitBySeatResult>, SplitBySeatCommandHandler>();
                    services.AddTransient<ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult>, CloseCashSessionCommandHandler>();
                    services.AddTransient<ICommandHandler<ChangeSeatCommand>, ChangeSeatCommandHandler>();
                    services.AddTransient<ICommandHandler<MergeTicketsCommand>, MergeTicketsCommandHandler>();
                    services.AddTransient<ICommandHandler<ChangeTableCommand, ChangeTableResult>, ChangeTableCommandHandler>();
                    services.AddTransient<ICommandHandler<SetCustomerCommand, SetCustomerResult>, SetCustomerCommandHandler>();
                    
                    // User Management (TECH-U003)
                    services.AddTransient<ICommandHandler<CreateUserCommand, CreateUserResult>, CreateUserCommandHandler>();
                    services.AddTransient<ICommandHandler<UpdateUserCommand, UpdateUserResult>, UpdateUserCommandHandler>();
                    services.AddTransient<ICommandHandler<DeleteUserCommand, DeleteUserResult>, DeleteUserCommandHandler>();
                    
                    services.AddTransient<ITableRepository, TableRepository>();
                    services.AddTransient<ITableLayoutRepository, TableLayoutRepository>();
                    services.AddTransient<IFloorRepository, FloorRepository>();

                    // Domain services
                    services.AddTransient<BatchPaymentDomainService>();
                    services.AddSingleton<IEventPublisher, EventPublisher>();

                    StartupLogger.Log("App - ConfigureServices End");
                })
                .Build();
            StartupLogger.Log("App - Host Building Success");
        } catch (Exception ex) {
            HandleFatalStartupError("Host Compilation Failed", ex);
            throw;
        }
        StartupLogger.Log("App Constructor - End");
    }

    private void HandleFatalStartupError(string stage, Exception ex)
    {
        var msg = $"CRITICAL STARTUP FAILURE\nStage: {stage}\nError: {ex.Message}\n\nStack:\n{ex.StackTrace}";
        StartupLogger.Log(msg);
        try 
        {
            MessageBox(IntPtr.Zero, msg, "Magidesk Fatal Error", 0x10); // 0x10 = MB_ICONHAND (Error)
        }
        catch 
        { 
             // T-001: Last Resort Fallback
             // If Native MessageBox fails (e.g. no window handle), write to desktop file.
             try
             {
                 var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                 var path = System.IO.Path.Combine(desktop, "MAGIDESK_FATAL_ERROR.txt");
                 System.IO.File.WriteAllText(path, msg + "\n\n(Native MessageBox Failed)");
             }
             catch { /* Absolute dead end */ }
        }
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        StartupLogger.Log("OnLaunched - Start");
        try
        {
            StartupLogger.Log("OnLaunched - Creating MainWindow");
            _window = new MainWindow();
            StartupLogger.Log("OnLaunched - MainWindow created");
            var mainWindow = (MainWindow)_window;
            MainWindowInstance = mainWindow;
            _window.Activate();

            System.Diagnostics.Debug.WriteLine("APP: Starting System Initialization...");
            StartupLogger.Log("OnLaunched - Showing Loading");
            mainWindow.ShowLoading("Initializing System...");

            StartupLogger.Log("OnLaunched - Resolving ISystemInitializationService");
            var initService = Host.Services.GetRequiredService<Magidesk.Application.Interfaces.ISystemInitializationService>();
            StartupLogger.Log("OnLaunched - Calling InitializeSystemAsync");
            var result = await initService.InitializeSystemAsync();

            if (result.IsSuccess)
            {
                StartupLogger.Log("OnLaunched - Initialization Successful");
                if (!string.IsNullOrEmpty(result.TerminalId))
                {
                    mainWindow.SetTerminalId(result.TerminalId);

                    var terminalContext = Host.Services.GetRequiredService<ITerminalContext>();
                    terminalContext.SetTerminalIdentity(result.TerminalId, result.TerminalGuid ?? Guid.Empty);
                }

                mainWindow.HideLoading();
                var navService = Host.Services.GetRequiredService<NavigationService>();
                
                StartupLogger.Log("OnLaunched - Navigating to LoginPage");
                navService.Navigate(typeof(Views.LoginPage));
                StartupLogger.Log("OnLaunched - Navigation success");
            }
            else
            {
                StartupLogger.Log($"OnLaunched - Initialization Failed: {result.Message}");
                mainWindow.ShowLoading($"Startup Failed: {result.Message}");
            }
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"APP FATAL: {ex}");
            try
            {
                if (_window == null)
                {
                    _window = new Window();
                    _window.Content = new Microsoft.UI.Xaml.Controls.TextBlock { 
                        Text = $"Fatal Startup Error:\n{ex.Message}\n\nStack:\n{ex.StackTrace}", 
                        Margin = new Thickness(20),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    };
                    _window.Activate();
                }
                else if (MainWindowInstance is MainWindow mw)
                {
                    mw.ShowLoading($"Critical Error: {ex.Message}");
                }
            }
            catch
            {
                // Last resort logging
            }
        }
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // PARANOID GLOBAL HANDLER (FEH-002) - UI THREAD
        HandleCriticalException(e.Exception, "UI_THREAD_UNHANDLED");
        // e.Handled = true; // DO NOT MARK HANDLED. Let it crash so the process restarts/closes properly.
    }

    // T-001: Global Background Exception Handler
    private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        HandleCriticalException(ex, "BACKGROUND_THREAD_UNHANDLED");
    }

    // T-001: Unobserved Task Exception Handler
    private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        HandleCriticalException(e.Exception, "UNOBSERVED_TASK_EXCEPTION");
        e.SetObserved(); // Mark observed to prevent immediate crash, but we still log and exit safely if needed.
        // Note: In .NET Core, unobserved task exceptions don't terminate process by default, 
        // but we want to know about them and potentially restart if critical.
        // For POS stability, we log heavily. If it's critical, we might want to warn user.
    }

    private void HandleCriticalException(Exception? ex, string source)
    {
        if (ex == null) return;

        // 1. Attempt Primary Logging
        try
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var logDir = System.IO.Path.Combine(appData, "Magidesk", "Logs");
            System.IO.Directory.CreateDirectory(logDir);
            var logPath = System.IO.Path.Combine(logDir, "crash_log.txt");
            
            var message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] CRITICAL FAILURE ({source}): {ex.Message}\nStack: {ex.StackTrace}\nInner: {ex.InnerException}\n--------------------------------\n";
            System.IO.File.AppendAllText(logPath, message);
        }
        catch (Exception logEx)
        {
            // 2. Fallback Logging (Console/Debug)
            System.Diagnostics.Debug.WriteLine($"[FATAL] Failed to write crash log: {logEx.Message}");
            System.Diagnostics.Debug.WriteLine($"With Original Exception: {ex}");
        }

        // 3. User Notification
        // Use native MessageBox because XAML might be corrupted or we are on bg thread.
        try
        {
            var errorMsg = $"A critical error occurred ({source}) and the application must close.\n\n" +
                          $"Error: {ex.Message}\n\n" +
                          $"The error has been logged to: crash_log.txt";
            
            MessageBox(IntPtr.Zero, errorMsg, "Magidesk Fatal Error", 0x10); // 0x10 = MB_ICONHAND | MB_OK
        }
        catch (Exception uiEx)
        {
            System.Diagnostics.Debug.WriteLine($"[FATAL] Failed to show error MessageBox: {uiEx.Message}");
        }

        // 4. Ensure Termination (if not already dying)
        // For UI exceptions, the runtime usually kills it. For AppDomain, it kills it.
        // For TaskScheduler, it might NOT kill it. We generally want to reset on critical state.
        // But for now, we just ensure visibility.
    }
}
