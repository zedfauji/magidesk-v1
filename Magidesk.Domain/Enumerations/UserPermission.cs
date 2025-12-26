using System;

namespace Magidesk.Domain.Enumerations;

[Flags]
public enum UserPermission
{
    None = 0,
    
    // Basic Operations
    CreateTicket = 1 << 0,
    EditTicket = 1 << 1,
    TakePayment = 1 << 2,
    
    // Manager Functions
    VoidTicket = 1 << 3,
    RefundPayment = 1 << 4,
    OpenDrawer = 1 << 5, // No sale
    CloseBatch = 1 << 6,
    ApplyDiscount = 1 << 7,
    
    // Admin Functions
    ManageUsers = 1 << 8,
    ManageTableLayout = 1 << 9,
    ManageMenu = 1 << 10,
    ViewReports = 1 << 11,
    SystemConfiguration = 1 << 12
    
    // All = ...
}
