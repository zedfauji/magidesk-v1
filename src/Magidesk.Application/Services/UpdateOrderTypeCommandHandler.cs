using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for UpdateOrderTypeCommand.
/// </summary>
public class UpdateOrderTypeCommandHandler : ICommandHandler<UpdateOrderTypeCommand, UpdateOrderTypeResult>
{
    private readonly IOrderTypeRepository _orderTypeRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public UpdateOrderTypeCommandHandler(
        IOrderTypeRepository orderTypeRepository,
        IAuditEventRepository auditEventRepository)
    {
        _orderTypeRepository = orderTypeRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<UpdateOrderTypeResult> HandleAsync(UpdateOrderTypeCommand command, CancellationToken cancellationToken = default)
    {
        // Get order type
        var orderType = await _orderTypeRepository.GetByIdAsync(command.OrderTypeId, cancellationToken);
        if (orderType == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Order type {command.OrderTypeId} not found.");
        }

        // Update order type properties
        if (command.Name != null)
        {
            orderType.UpdateName(command.Name);
        }

        if (command.CloseOnPaid.HasValue)
        {
            orderType.SetCloseOnPaid(command.CloseOnPaid.Value);
        }

        if (command.AllowSeatBasedOrder.HasValue)
        {
            orderType.SetAllowSeatBasedOrder(command.AllowSeatBasedOrder.Value);
        }

        if (command.AllowToAddTipsLater.HasValue)
        {
            orderType.SetAllowToAddTipsLater(command.AllowToAddTipsLater.Value);
        }

        if (command.IsBarTab.HasValue)
        {
            orderType.SetIsBarTab(command.IsBarTab.Value);
        }

        if (command.IsActive.HasValue)
        {
            if (command.IsActive.Value)
            {
                orderType.Activate();
            }
            else
            {
                orderType.Deactivate();
            }
        }

        // Update order type
        await _orderTypeRepository.UpdateAsync(orderType, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(OrderType),
            orderType.Id,
            Guid.Empty, // System operation
            System.Text.Json.JsonSerializer.Serialize(new
            {
                Name = orderType.Name,
                CloseOnPaid = orderType.CloseOnPaid,
                AllowSeatBasedOrder = orderType.AllowSeatBasedOrder,
                AllowToAddTipsLater = orderType.AllowToAddTipsLater,
                IsBarTab = orderType.IsBarTab,
                IsActive = orderType.IsActive
            }),
            $"Order type '{orderType.Name}' updated",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new UpdateOrderTypeResult
        {
            OrderTypeId = orderType.Id,
            Name = orderType.Name
        };
    }
}

