using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for PrintReceiptCommand.
/// </summary>
public class PrintReceiptCommandHandler : ICommandHandler<PrintReceiptCommand, PrintReceiptResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IReceiptPrintService _receiptPrintService;
    private readonly IAuditEventRepository _auditEventRepository;

    public PrintReceiptCommandHandler(
        ITicketRepository ticketRepository,
        IReceiptPrintService receiptPrintService,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _receiptPrintService = receiptPrintService;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<PrintReceiptResult> HandleAsync(PrintReceiptCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        bool success = false;

        if (command.ReceiptType == ReceiptType.Ticket || !command.PaymentId.HasValue)
        {
            // Print ticket receipt
            success = await _receiptPrintService.PrintTicketReceiptAsync(ticket, cancellationToken);
        }
        else if (command.PaymentId.HasValue)
        {
            // Get payment
            var payment = ticket.Payments.FirstOrDefault(p => p.Id == command.PaymentId.Value);
            if (payment == null)
            {
                throw new Domain.Exceptions.BusinessRuleViolationException($"Payment {command.PaymentId.Value} not found.");
            }

            if (command.ReceiptType == ReceiptType.Refund && payment.TransactionType == Domain.Enumerations.TransactionType.Debit)
            {
                // Print refund receipt
                success = await _receiptPrintService.PrintRefundReceiptAsync(payment, ticket, cancellationToken);
            }
            else
            {
                // Print payment receipt
                success = await _receiptPrintService.PrintPaymentReceiptAsync(payment, ticket, cancellationToken);
            }
        }

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            Domain.Enumerations.AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            Guid.Empty, // System operation
            System.Text.Json.JsonSerializer.Serialize(new
            {
                ReceiptType = command.ReceiptType,
                PaymentId = command.PaymentId,
                Success = success
            }),
            $"Printed {command.ReceiptType} receipt for ticket #{ticket.TicketNumber}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new PrintReceiptResult
        {
            Success = success
        };
    }
}

