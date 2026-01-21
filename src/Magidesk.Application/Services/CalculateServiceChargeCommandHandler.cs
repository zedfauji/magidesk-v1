using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.Entities;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for CalculateServiceChargeCommand.
/// Calculates service charge based on percentage and sets it on the ticket.
/// </summary>
public class CalculateServiceChargeCommandHandler : ICommandHandler<CalculateServiceChargeCommand, CalculateServiceChargeResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly ServiceChargeDomainService _serviceChargeDomainService;

    public CalculateServiceChargeCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository,
        ServiceChargeDomainService serviceChargeDomainService)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
        _serviceChargeDomainService = serviceChargeDomainService;
    }

    public async Task<CalculateServiceChargeResult> HandleAsync(
        CalculateServiceChargeCommand command,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            return new CalculateServiceChargeResult
            {
                Success = false,
                ErrorMessage = $"Ticket {command.TicketId} not found."
            };
        }

        try
        {
            // Calculate service charge
            var calculatedAmount = _serviceChargeDomainService.CalculateServiceChargeForTicket(
                ticket,
                command.ServiceChargeRate);

            // Set the calculated amount
            ticket.SetServiceCharge(calculatedAmount);

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            // Create audit event
            var auditEvent = AuditEvent.Create(
                AuditEventType.Modified,
                nameof(Ticket),
                ticket.Id,
                command.ProcessedBy.Value,
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    Action = "CalculateServiceCharge",
                    ServiceChargeRate = command.ServiceChargeRate,
                    CalculatedAmount = calculatedAmount
                }),
                $"Service charge calculated ({command.ServiceChargeRate * 100}%): {calculatedAmount} on ticket {ticket.TicketNumber}",
                correlationId: Guid.NewGuid());

            await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

            return new CalculateServiceChargeResult
            {
                Success = true,
                CalculatedAmount = calculatedAmount,
                NewTotalAmount = ticket.TotalAmount
            };
        }
        catch (Exception ex)
        {
            return new CalculateServiceChargeResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}

