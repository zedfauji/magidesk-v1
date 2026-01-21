using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Printing.Models;

namespace Magidesk.Infrastructure.Printing.Drivers;

public interface IPrintDriver
{
    byte[] Render(PrintDocument doc, PrinterMapping mapping);
}
