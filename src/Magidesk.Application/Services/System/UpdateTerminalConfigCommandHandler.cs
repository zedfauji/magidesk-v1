using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services.SystemConfig;

public class UpdateTerminalConfigCommandHandler : ICommandHandler<UpdateTerminalConfigCommand, UpdateTerminalConfigResult>
{
    private readonly ITerminalRepository _repository;

    public UpdateTerminalConfigCommandHandler(ITerminalRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateTerminalConfigResult> HandleAsync(UpdateTerminalConfigCommand command, CancellationToken cancellationToken = default)
    {
        var dto = command.Terminal;
        
        // We need to fetch the existing entity to ensure we have the correct ID and immutable fields
        var terminal = await _repository.GetByTerminalKeyAsync(dto.TerminalKey, cancellationToken);
        
        if (terminal == null)
        {
            return new UpdateTerminalConfigResult(false, "Terminal not found.");
        }

        terminal.Name = dto.Name;
        terminal.Location = dto.Location;
        terminal.FloorId = dto.FloorId;
        terminal.HasCashDrawer = dto.HasCashDrawer;
        terminal.OpeningBalance = dto.OpeningBalance;
        terminal.CurrentBalance = dto.CurrentBalance;
        terminal.AutoLogOut = dto.AutoLogOut;
        terminal.AutoLogOutTime = dto.AutoLogOutTime;
        terminal.ShowGuestSelection = dto.ShowGuestSelection;
        terminal.ShowTableSelection = dto.ShowTableSelection;
        terminal.KitchenMode = dto.KitchenMode;
        terminal.DefaultFontSize = dto.DefaultFontSize;
        terminal.DefaultFontFamily = dto.DefaultFontFamily;

        await _repository.UpdateTerminalAsync(terminal, cancellationToken);
        
        return new UpdateTerminalConfigResult(true, "Terminal configuration updated successfully.");
    }
}
