using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for ApplyGratuityCommand.
/// Applies gratuity/tips to a ticket using the domain service.
/// Uses IServiceScopeFactory to create fresh DbContext per retry attempt,
/// ensuring proper EF Core lifetime management during concurrency retries.
/// </summary>
public class ApplyGratuityCommandHandler : ICommandHandler<ApplyGratuityCommand, ApplyGratuityResult>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ApplyGratuityCommandHandler> _logger;

    public ApplyGratuityCommandHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<ApplyGratuityCommandHandler> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
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

            int maxRetries = 3;
            for (int i = 0; i < maxRetries; i++)
            {
                _logger.LogInformation(
                    "[DIAGNOSTIC] Starting gratuity attempt {Attempt}/{MaxRetries} for ticket {TicketId}",
                    i + 1, maxRetries, command.TicketId);

                // Create a fresh DI scope for each retry attempt
                // This ensures a new DbContext instance is created, respecting EF Core lifetime rules
                using (var scope = _scopeFactory.CreateScope())
                {
                    try
                    {
                        // Resolve fresh instances from the new scope
                        var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                        var gratuityService = scope.ServiceProvider.GetRequiredService<IGratuityService>();

                        _logger.LogInformation(
                            "[DIAGNOSTIC] Fresh scope created for attempt {Attempt}. Resolving ticket...",
                            i + 1);

                        // Get ticket (fresh load with new DbContext)
                        var ticket = await ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
                        if (ticket == null)
                        {
                            _logger.LogWarning(
                                "[DIAGNOSTIC] Ticket {TicketId} not found in database",
                                command.TicketId);
                            return new ApplyGratuityResult
                            {
                                Success = false,
                                ErrorMessage = $"Ticket {command.TicketId} not found."
                            };
                        }

                        // Log ticket state BEFORE applying gratuity
                        _logger.LogInformation(
                            "[DIAGNOSTIC] Ticket loaded - ID: {TicketId}, Version: {Version}, Status: {Status}, HasGratuity: {HasGratuity}",
                            ticket.Id, ticket.Version, ticket.Status, ticket.Gratuity != null);

                        // Apply gratuity using domain service
                        gratuityService.ApplyGratuity(ticket, command.Amount, command.ServerId);

                        _logger.LogInformation(
                            "[DIAGNOSTIC] Gratuity applied in memory. Attempting to save...");

                        // Save changes
                        await ticketRepository.UpdateAsync(ticket, cancellationToken);

                        _logger.LogInformation(
                            "[DIAGNOSTIC] Save successful! Gratuity {Amount} applied to ticket {TicketId} by {ProcessedBy}",
                            command.Amount,
                            command.TicketId,
                            command.ProcessedBy);

                        return new ApplyGratuityResult
                        {
                            Success = true,
                            GratuityId = ticket.Gratuity?.Id
                        };
                    }
                    catch (Magidesk.Domain.Exceptions.ConcurrencyException ex)
                    {
                        _logger.LogWarning(
                            "[DIAGNOSTIC] Concurrency exception caught on attempt {Attempt}/{MaxRetries}. Exception: {Message}",
                            i + 1, maxRetries, ex.Message);

                        if (i == maxRetries - 1)
                        {
                            _logger.LogError(ex, 
                                "[DIAGNOSTIC] Max retries reached. Concurrency conflict applying gratuity to ticket {TicketId}.", 
                                command.TicketId);
                            throw; 
                        }

                        _logger.LogWarning(
                            "[DIAGNOSTIC] Will retry... Disposing current scope and creating fresh DbContext for attempt {NextAttempt}",
                            i + 2);
                        
                        // No need to clear change tracker - the scope (and DbContext) will be disposed
                        // at the end of this using block, and a fresh one will be created in the next iteration
                    }
                } // DbContext automatically disposed here when scope is disposed
            }
            
            throw new InvalidOperationException("Unreachable code");
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
