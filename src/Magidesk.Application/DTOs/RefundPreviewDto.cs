using System;
using System.Collections.Generic;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.DTOs
{
    public class RefundPreviewDto
    {
        public Guid TicketId { get; set; }
        
        // Before Snapshot
        public Money OriginalTotalAmount { get; set; } = Money.Zero();
        public Money OriginalPaidAmount { get; set; } = Money.Zero();
        public Money OriginalDueAmount { get; set; } = Money.Zero();

        // After Snapshot
        public Money ProjectedPaidAmount { get; set; } = Money.Zero();
        public Money ProjectedDueAmount { get; set; } = Money.Zero();
        
        // Actions
        public List<ProposedRefundAction> ProposedActions { get; set; } = new();
        public bool IsFullRefund { get; set; }
        public string ValidationWarning { get; set; } = string.Empty;
    }

    public class ProposedRefundAction
    {
        public string Description { get; set; } = string.Empty;
        public Money Amount { get; set; } = Money.Zero();
        public string PaymentMethod { get; set; } = string.Empty;
        public bool IsGatewayRefund { get; set; }
    }
}
