using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.Services;
using Magidesk.Application.Interfaces;

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
        
        // Handlers without results
        services.AddScoped<ICommandHandler<Commands.RemoveOrderLineCommand>, RemoveOrderLineCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ModifyOrderLineCommand>, ModifyOrderLineCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.ApplyDiscountCommand>, ApplyDiscountCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.CloseTicketCommand>, CloseTicketCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.VoidTicketCommand>, VoidTicketCommandHandler>();

        // Register query handlers
        services.AddScoped<IQueryHandler<Queries.GetTicketQuery, DTOs.TicketDto>, GetTicketQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetTicketByNumberQuery, DTOs.TicketDto>, GetTicketByNumberQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetOpenTicketsQuery, IEnumerable<DTOs.TicketDto>>, GetOpenTicketsQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetCurrentCashSessionQuery, Queries.GetCurrentCashSessionResult>, GetCurrentCashSessionQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetCashSessionQuery, Queries.GetCashSessionResult>, GetCashSessionQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.GetDrawerPullReportQuery, Queries.GetDrawerPullReportResult>, GetDrawerPullReportQueryHandler>();
        // GetMenuItemsQuery handler will be implemented in Phase 2 when menu management is added

        return services;
    }
}

