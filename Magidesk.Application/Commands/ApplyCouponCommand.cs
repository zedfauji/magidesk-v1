using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to apply a coupon by code.
/// </summary>
public class ApplyCouponCommand
{
    public Guid TicketId { get; set; }
    public string CouponCode { get; set; } = string.Empty;
    public UserId AppliedBy { get; set; } = null!;
}
