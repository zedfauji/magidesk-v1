using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for ApplyGratuityCommand.
/// Applies gratuity/tips to a ticket using the domain service.
/// </summary>
public class ApplyGratuityCommandHandler : ICommandHandler<ApplyGratuityCommand, ApplyGratuityResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IGratuityService _gratuityService;
    private readonly ILogger<ApplyGratuityCommandHandler> _logger;

    public ApplyGratuityCommandHandler(
        ITicketRepository ticketRepository,
        IGratuityService gratuityService,
        ILogger<ApplyGratuityCommandHandler> logger)
    {
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _gratuityService = gratuityService ?? throw new ArgumentNullException(nameof(gratuityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplyGratuityResult> HandleAsync(
        ApplyGratuityCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate command
            if (command.Amount == null)
            {
                return new ApplyGratuityResult
                {
                    Success = false,
                    ErrorMessage = "Gratuity amount is required."
                };
            }

            // Get ticket
            var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
            if (ticket == null)
            {
                return new ApplyGratuityResult
                {
                    Success = false,
                    ErrorMessage = $"Ticket {command.TicketId} not found."
                };
            }

            // Apply gratuity using domain service
            _gratuityService.ApplyGratuity(ticket, command.Amount, command.ServerId);

            // Save changes
            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            _logger.LogInformation(
                "Gratuity {Amount} applied to ticket {TicketId} by {ProcessedBy}",
                command.Amount,
                command.TicketId,
                command.ProcessedBy);

            return new ApplyGratuityResult
            {
                Success = true,
                GratuityId = ticket.Gratuity?.Id
            };
        }
        catch (Domain.Exceptions.BusinessRuleViolationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation applying gratuity to ticket {TicketId}", command.TicketId);
            return new ApplyGratuityResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying gratuity to ticket {TicketId}", command.TicketId);
            return new ApplyGratuityResult
            {
                Success = false,
                ErrorMessage = $"Failed to apply gratuity: {ex.Message}"
            };
        }
    }
}
