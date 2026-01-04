using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Commands;

public class OpenCashDrawerCommand
{
    public string PrinterName { get; set; } = string.Empty;
}

public class OpenCashDrawerCommandHandler : ICommandHandler<OpenCashDrawerCommand>
{
    private readonly ICashDrawerService _cashDrawerService;

    public OpenCashDrawerCommandHandler(ICashDrawerService cashDrawerService)
    {
        _cashDrawerService = cashDrawerService;
    }

    public async Task HandleAsync(OpenCashDrawerCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.PrinterName))
        {
            // TODO: Fallback to Terminal's Default 'Receipt' Printer if empty
            // For now, require it
            throw new ArgumentException("Printer Name is required to open drawer.");
        }
        
        await _cashDrawerService.OpenDrawerAsync(command.PrinterName);
    }
}
