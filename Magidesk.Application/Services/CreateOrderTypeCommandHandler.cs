using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for CreateOrderTypeCommand.
/// </summary>
public class CreateOrderTypeCommandHandler : ICommandHandler<CreateOrderTypeCommand, CreateOrderTypeResult>
{
    private readonly IOrderTypeRepository _orderTypeRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public CreateOrderTypeCommandHandler(
        IOrderTypeRepository orderTypeRepository,
        IAuditEventRepository auditEventRepository)
    {
        _orderTypeRepository = orderTypeRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<CreateOrderTypeResult> HandleAsync(CreateOrderTypeCommand command, CancellationToken cancellationToken = default)
    {
        // Create order type
        var orderType = OrderType.Create(
            command.Name,
            command.CloseOnPaid,
            command.AllowSeatBasedOrder,
            command.AllowToAddTipsLater,
            command.IsBarTab,
            command.IsActive);

        // Save order type
        await _orderTypeRepository.AddAsync(orderType, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Created,
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
            $"Order type '{orderType.Name}' created",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new CreateOrderTypeResult
        {
            OrderTypeId = orderType.Id,
            Name = orderType.Name
        };
    }
}

