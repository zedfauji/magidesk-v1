using System;
using System.Collections.Generic;

namespace Magidesk.Application.Commands;

public class AddOrderLineComboCommand
{
    public Guid TicketId { get; set; }
    public Guid OrderLineId { get; set; }
    public List<ComboSelection> Selections { get; set; } = new();

    public class ComboSelection
    {
        public Guid ComboGroupId { get; set; }
        public Guid ComboGroupItemId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
