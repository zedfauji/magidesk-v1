using System;
using System.Threading.Tasks;

namespace Magidesk.Application.Interfaces;

public interface IKitchenStatusService
{
    /// <summary>
    /// Bumps the kitchen order to the next status (New -> Cooking -> Done).
    /// </summary>
    Task BumpOrderAsync(Guid kitchenOrderId);

    /// <summary>
    /// Voids the kitchen order.
    /// </summary>
    Task VoidOrderAsync(Guid kitchenOrderId);
}
