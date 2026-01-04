using System;
using System.Threading.Tasks;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Interfaces.Printing;
using Magidesk.Infrastructure.Printing.Layouts;

namespace Magidesk.Infrastructure.Printing.Engines
{
    public class PrintLayoutEngine : IPrintLayoutEngine
    {
        private readonly Func<PrinterFormat, IPrintLayoutAdapter> _adapterFactory;

        public PrintLayoutEngine(Func<PrinterFormat, IPrintLayoutAdapter> adapterFactory)
        {
            _adapterFactory = adapterFactory;
        }

        public async Task<string> GenerateTicketLayoutAsync(object ticketData, PrinterFormat format)
        {
            var adapter = _adapterFactory(format);
            if (adapter == null)
            {
                throw new NotSupportedException($"No layout adapter found for format: {format}");
            }

            return await adapter.GenerateLayoutAsync(ticketData);
        }
    }
}
