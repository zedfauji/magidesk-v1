using System;
using System.Threading.Tasks;

namespace Magidesk.Application.Interfaces;

public interface ILowStockAlertService
{
    Task CheckAndAlertLowStock(Guid menuItemId);
}
