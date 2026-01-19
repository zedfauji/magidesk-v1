using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for RefundTicketCommand.
/// REQ-5.4, REQ-5.5, REQ-5.6, REQ-5.7, REQ-5.9: Validates authorization, processes refunds, and generates receipts.
/// </summary>
public class RefundTicketCommandHandler : ICommandHandler<RefundTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly ISecurityService _securityService;
    private readonly IReceiptPrintService _receiptPrintService;

    public RefundTicketCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository,
        ISecurityService securityService,
        IReceiptPrintService receiptPrintService)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
        _securityService = securityService;
        _receiptPrintService = receiptPrintService;
    }

    public async Task HandleAsync(RefundTicketCommand command, CancellationToken cancellationToken = default)
    {
        // Validate command
        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new Domain.Exceptions.BusinessRuleViolationException("Refund reason is required.");
        }

        if (command.Amount == null || command.Amount <= Money.Zero())
        {
            throw new Domain.Exceptions.BusinessRuleViolationException("Refund amount must be greater than zero.");
        }

        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // REQ-5.6: Validate manager authorization
        // Check if the authorizing user has manager permissions
        if (!await _securityService.HasPermissionAsync(command.AuthorizedBy, UserPermission.RefundTicket, cancellationToken))
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(
                "Manager authorization is required to refund tickets. The authorizing user does not have RefundTicket permission.");
        }

        // REQ-5.9: Validate refund amount doesn't exceed paid amount
        if (command.Amount > ticket.PaidAmount)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(
                $"Refund amount ({command.Amount}) cannot exceed paid amount ({ticket.PaidAmount}).");
        }

        // Determine if this is a full or partial refund
        var isFullRefund = command.Amount >= ticket.PaidAmount;

        // REQ-5.4, REQ-5.5: Process refund
        try
        {
            ticket.Refund(command.Amount, command.Reason, command.RefundedBy);
        }
        catch (Domain.Exceptions.InvalidOperationException ex)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(ex.Message, ex);
        }

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // REQ-5.8: Create audit event
        var correlationId = Guid.NewGuid();
        var refundType = isFullRefund ? "Full" : "Partial";
        var auditEvent = AuditEvent.Create(
            AuditEventType.Refunded,
            nameof(Ticket),
            ticket.Id,
            command.RefundedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new 
            { 
                Status = ticket.Status.ToString(),
                RefundAmount = command.Amount.Amount,
                RefundType = refundType,
                Reason = command.Reason,
                RefundedBy = command.RefundedBy.Value,
                AuthorizedBy = command.AuthorizedBy.Value,
                RemainingPaidAmount = ticket.PaidAmount.Amount
            }),
            $"Ticket #{ticket.TicketNumber} {refundType.ToLower()} refund of {command.Amount} processed by {command.RefundedBy.Value}, authorized by {command.AuthorizedBy.Value}. Reason: {command.Reason}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        // REQ-5.7: Generate refund receipt
        // Find the most recent refund payment (debit transaction) for this refund
        var refundPayment = ticket.Payments
            .Where(p => p.TransactionType == TransactionType.Debit)
            .OrderByDescending(p => p.TransactionTime)
            .FirstOrDefault();

        if (refundPayment != null)
        {
            try
            {
                await _receiptPrintService.PrintRefundReceiptAsync(
                    refundPayment, 
                    ticket, 
                    command.RefundedBy.Value, 
                    cancellationToken);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the refund operation
                // The refund has already been processed successfully
                var printErrorEvent = AuditEvent.Create(
                    AuditEventType.Modified,
                    nameof(Ticket),
                    ticket.Id,
                    command.RefundedBy.Value,
                    System.Text.Json.JsonSerializer.Serialize(new { Error = ex.Message }),
                    $"Failed to print refund receipt for ticket #{ticket.TicketNumber}: {ex.Message}",
                    correlationId: correlationId);

                await _auditEventRepository.AddAsync(printErrorEvent, cancellationToken);
            }
        }
    }
}
