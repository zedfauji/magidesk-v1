using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Magidesk.Application.Services;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Commands;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.Queries.SystemConfig;
using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Services.SystemConfig;

namespace Magidesk.Application.DependencyInjection;

/// <summary>
/// Extension methods for configuring dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Application layer services to the service collection.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        });

        // Register command handlers
        // Handlers with results
        services.AddScoped<ICommandHandler<Commands.CreateTicketCommand, Commands.CreateTicketResult>, CreateTicketCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.AddOrderLineCommand, Commands.AddOrderLineResult>, AddOrderLineCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ProcessPaymentCommand, Commands.ProcessPaymentResult>, ProcessPaymentCommandHandler>();
        
        // Card payment command handlers
        services.AddScoped<ICommandHandler<Commands.AuthorizeCardPaymentCommand, Commands.AuthorizeCardPaymentResult>, AuthorizeCardPaymentCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.CaptureCardPaymentCommand, Commands.CaptureCardPaymentResult>, CaptureCardPaymentCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.VoidCardPaymentCommand, Commands.VoidCardPaymentResult>, VoidCardPaymentCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.AddTipsToCardPaymentCommand, Commands.AddTipsToCardPaymentResult>, AddTipsToCardPaymentCommandHandler>();
        
        // Refund command handlers
        services.AddScoped<ICommandHandler<Commands.RefundPaymentCommand, Commands.RefundPaymentResult>, RefundPaymentCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.RefundTicketCommand, Commands.RefundTicketResult>, RefundTicketCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.SplitTicketCommand, Commands.SplitTicketResult>, SplitTicketCommandHandler>();
        
        // Service charges and adjustments command handlers
        services.AddScoped<ICommandHandler<Commands.SetServiceChargeCommand, Commands.SetServiceChargeResult>, SetServiceChargeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.CalculateServiceChargeCommand, Commands.CalculateServiceChargeResult>, CalculateServiceChargeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.SetDeliveryChargeCommand, Commands.SetDeliveryChargeResult>, SetDeliveryChargeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.SetAdjustmentCommand, Commands.SetAdjustmentResult>, SetAdjustmentCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.SetAdvancePaymentCommand, Commands.SetAdvancePaymentResult>, SetAdvancePaymentCommandHandler>();
        
        // Cash session command handlers
        services.AddScoped<ICommandHandler<Commands.OpenCashSessionCommand, Commands.OpenCashSessionResult>, OpenCashSessionCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.CloseCashSessionCommand, Commands.CloseCashSessionResult>, CloseCashSessionCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.AddPayoutCommand, Commands.AddPayoutResult>, AddPayoutCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.AddCashDropCommand, Commands.AddCashDropResult>, AddCashDropCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.AddDrawerBleedCommand, Commands.AddDrawerBleedResult>, AddDrawerBleedCommandHandler>();
        
        // Shift command handlers
        services.AddScoped<ICommandHandler<Commands.CreateShiftCommand, Commands.CreateShiftResult>, CreateShiftCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.UpdateShiftCommand, Commands.UpdateShiftResult>, UpdateShiftCommandHandler>();
        
        // Order type command handlers
        services.AddScoped<ICommandHandler<Commands.CreateOrderTypeCommand, Commands.CreateOrderTypeResult>, CreateOrderTypeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.UpdateOrderTypeCommand, Commands.UpdateOrderTypeResult>, UpdateOrderTypeCommandHandler>();
        
        // Table command handlers
        services.AddScoped<ICommandHandler<Commands.CreateTableCommand, Commands.CreateTableResult>, CreateTableCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.UpdateTableCommand, Commands.UpdateTableResult>, UpdateTableCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.AssignTableToTicketCommand, Commands.AssignTableToTicketResult>, AssignTableToTicketCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.TransferTicketToTableCommand, Commands.TransferTicketToTableResult>, TransferTicketToTableCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ReleaseTableCommand, Commands.ReleaseTableResult>, ReleaseTableCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ChangeTableCommand, Commands.ChangeTableResult>, ChangeTableCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.SetTaxExemptCommand, Commands.SetTaxExemptResult>, SetTaxExemptCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ChangeTicketOrderTypeCommand>, ChangeTicketOrderTypeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.SetTicketCustomerCommand>, SetTicketCustomerCommandHandler>();
        
        // Delivery command handlers
        services.AddScoped<ICommandHandler<Commands.MarkTicketAsReadyCommand>, MarkTicketAsReadyCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.DispatchTicketCommand>, DispatchTicketCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ScheduleTicketCommand>, ScheduleTicketCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.FireScheduledTicketsCommand>, FireScheduledTicketsCommandHandler>();
        
        // Print command handlers
        services.AddScoped<ICommandHandler<Commands.PrintToKitchenCommand, Commands.PrintToKitchenResult>, PrintToKitchenCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.PrintReceiptCommand, Commands.PrintReceiptResult>, PrintReceiptCommandHandler>();
        
        // Handlers without results
        services.AddScoped<ICommandHandler<Commands.RemoveOrderLineCommand>, RemoveOrderLineCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ModifyOrderLineCommand>, ModifyOrderLineCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.AddOrderLineModifierCommand>, AddOrderLineModifierCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.RemoveOrderLineModifierCommand>, RemoveOrderLineModifierCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.AddOrderLineInstructionCommand>, AddOrderLineInstructionCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ApplyDiscountCommand>, ApplyDiscountCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ApplyCouponCommand>, ApplyCouponCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.AddOrderLineComboCommand>, AddOrderLineComboCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.CloseTicketCommand>, CloseTicketCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.UpdateTicketNoteCommand>, UpdateTicketNoteCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.UpdateOrderLineInstructionCommand>, UpdateOrderLineInstructionCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.VoidTicketCommand>, VoidTicketCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.PayNowCommand>, PayNowCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.LogoutCommand>, LogoutCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.SettleTicketCommand>, SettleTicketCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ClockInCommand>, ClockInCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ClockOutCommand>, ClockOutCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.TransferTicketCommand>, TransferTicketCommandHandler>();

        // Register query handlers
        services.AddScoped<GetTicketQueryHandler>(); // Concrete class needed by other handlers
        services.AddScoped<IQueryHandler<Queries.GetTicketQuery, DTOs.TicketDto?>, GetTicketQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetTicketByNumberQuery, DTOs.TicketDto?>, GetTicketByNumberQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetOpenTicketsQuery, IEnumerable<DTOs.TicketDto>>, GetOpenTicketsQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetCurrentCashSessionQuery, Queries.GetCurrentCashSessionResult>, GetCurrentCashSessionQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetCashSessionQuery, Queries.GetCashSessionResult>, GetCashSessionQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetDrawerPullReportQuery, Queries.GetDrawerPullReportResult>, GetDrawerPullReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetShiftQuery, Queries.GetShiftResult>, GetShiftQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetCurrentShiftQuery, Queries.GetCurrentShiftResult>, GetCurrentShiftQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetShiftReportQuery, Queries.GetShiftReportResult>, GetShiftReportQueryHandler>();
        
        // Order type query handlers
        services.AddScoped<IQueryHandler<Queries.GetOrderTypeQuery, Queries.GetOrderTypeResult>, GetOrderTypeQueryHandler>();
        
        // Table query handlers
        services.AddScoped<IQueryHandler<Queries.GetTableQuery, Queries.GetTableResult>, GetTableQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetAvailableTablesQuery, Queries.GetAvailableTablesResult>, GetAvailableTablesQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetTableMapQuery, Queries.GetTableMapResult>, GetTableMapQueryHandler>();

        services.AddScoped<IQueryHandler<Queries.GetSalesBalanceQuery, DTOs.Reports.SalesBalanceReportDto>, Services.Reports.GetSalesBalanceQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetSalesSummaryQuery, DTOs.Reports.SalesSummaryReportDto>, Services.Reports.GetSalesSummaryQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetExceptionsReportQuery, DTOs.Reports.ExceptionsReportDto>, Services.Reports.GetExceptionsReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetJournalReportQuery, DTOs.Reports.JournalReportDto>, Services.Reports.GetJournalReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetProductivityReportQuery, DTOs.Reports.ProductivityReportDto>, Services.Reports.GetProductivityReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetLaborReportQuery, DTOs.Reports.LaborReportDto>, Services.Reports.GetLaborReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetDeliveryReportQuery, DTOs.Reports.DeliveryReportDto>, Services.Reports.GetDeliveryReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetSalesDetailQuery, DTOs.Reports.SalesDetailReportDto>, Services.Reports.GetSalesDetailQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetCreditCardReportQuery, DTOs.Reports.CreditCardReportDto>, Services.Reports.GetCreditCardReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetMenuUsageReportQuery, DTOs.Reports.MenuUsageReportDto>, Services.Reports.GetMenuUsageReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetPaymentReportQuery, DTOs.Reports.PaymentReportDto>, Services.Reports.GetPaymentReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetServerProductivityReportQuery, DTOs.Reports.ServerProductivityReportDto>, Services.Reports.GetServerProductivityReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetHourlyLaborReportQuery, DTOs.Reports.HourlyLaborReportDto>, Services.Reports.GetHourlyLaborReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetTipReportQuery, DTOs.Reports.TipReportDto>, Services.Reports.GetTipReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetAttendanceReportQuery, DTOs.Reports.AttendanceReportDto>, Services.Reports.GetAttendanceReportQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reports.GetCashOutReportQuery, DTOs.Reports.CashOutReportDto>, Services.Reports.GetCashOutReportQueryHandler>();
        
        // System Config query handlers
        services.AddScoped<IQueryHandler<GetSystemBackupsQuery, GetSystemBackupsResult>, GetSystemBackupsQueryHandler>();

        // System Config command handlers
        services.AddScoped<ICommandHandler<CreateSystemBackupCommand, CreateSystemBackupResult>, CreateSystemBackupCommandHandler>();
        services.AddScoped<ICommandHandler<RestoreSystemBackupCommand, RestoreSystemBackupResult>, RestoreSystemBackupCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateRestaurantConfigCommand, UpdateRestaurantConfigResult>, UpdateRestaurantConfigCommandHandler>();
        services.AddScoped<IQueryHandler<GetRestaurantConfigQuery, GetRestaurantConfigResult>, GetRestaurantConfigQueryHandler>();
        services.AddScoped<IQueryHandler<GetTerminalConfigQuery, GetTerminalConfigResult>, GetTerminalConfigQueryHandler>();
        services.AddScoped<ICommandHandler<UpdateTerminalConfigCommand, UpdateTerminalConfigResult>, UpdateTerminalConfigCommandHandler>();
        services.AddScoped<IQueryHandler<GetCardConfigQuery, GetCardConfigResult>, GetCardConfigQueryHandler>();
        services.AddScoped<ICommandHandler<UpdateCardConfigCommand, UpdateCardConfigResult>, UpdateCardConfigCommandHandler>();
        services.AddScoped<IQueryHandler<GetPrinterGroupsQuery, GetPrinterGroupsResult>, GetPrinterGroupsQueryHandler>();
        services.AddScoped<IQueryHandler<GetPrinterMappingsQuery, GetPrinterMappingsResult>, GetPrinterMappingsQueryHandler>();
        services.AddScoped<ICommandHandler<UpdatePrinterGroupsCommand, UpdatePrinterGroupsResult>, UpdatePrinterGroupsCommandHandler>();
        services.AddScoped<ICommandHandler<UpdatePrinterMappingsCommand, UpdatePrinterMappingsResult>, UpdatePrinterMappingsCommandHandler>();

        // User Management Handlers
        services.AddScoped<ICommandHandler<CreateUserCommand, CreateUserResult>, CreateUserCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand, UpdateUserResult>, UpdateUserCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteUserCommand, DeleteUserResult>, DeleteUserCommandHandler>();

        // Role Management Handlers
        services.AddScoped<ICommandHandler<CreateRoleCommand, CreateRoleResult>, CreateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateRoleCommand, UpdateRoleResult>, UpdateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteRoleCommand, DeleteRoleResult>, DeleteRoleCommandHandler>();

        services.AddScoped<IKitchenRoutingService, KitchenRoutingService>();
        services.AddScoped<IKitchenStatusService, KitchenStatusService>();
        services.AddScoped<ICashSessionService, CashSessionService>();
        services.AddScoped<IGroupSettleService, GroupSettleService>();
        services.AddScoped<IMerchantBatchService, MerchantBatchService>();
        services.AddScoped<TableLayoutExporter>();

        return services;
    }
}
