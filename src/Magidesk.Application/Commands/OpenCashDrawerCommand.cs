using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Commands;

public class OpenCashDrawerCommand
{
    public string? PrinterName { get; set; }
}

public class OpenCashDrawerCommandHandler : ICommandHandler<OpenCashDrawerCommand>
{
    private readonly ICashDrawerService _cashDrawerService;
    private readonly IReceiptPrintService _receiptPrintService;

    public OpenCashDrawerCommandHandler(ICashDrawerService cashDrawerService, IReceiptPrintService receiptPrintService)
    {
        _cashDrawerService = cashDrawerService;
        _receiptPrintService = receiptPrintService;
    }

    public async Task HandleAsync(OpenCashDrawerCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.PrinterName))
        {
            // Use service to resolve terminal's default printer
            await _receiptPrintService.OpenCashDrawerAsync(cancellationToken);
        }
        else
        {
            await _cashDrawerService.OpenDrawerAsync(command.PrinterName);
        }
    }
}
