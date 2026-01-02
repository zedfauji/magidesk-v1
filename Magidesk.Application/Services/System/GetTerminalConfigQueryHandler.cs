using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Queries.SystemConfig;

namespace Magidesk.Application.Services.SystemConfig;

public class GetTerminalConfigQueryHandler : IQueryHandler<GetTerminalConfigQuery, GetTerminalConfigResult>
{
    private readonly ITerminalRepository _repository;

    public GetTerminalConfigQueryHandler(ITerminalRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetTerminalConfigResult> HandleAsync(GetTerminalConfigQuery query, CancellationToken cancellationToken = default)
    {
        var terminal = await _repository.GetByTerminalKeyAsync(query.TerminalKey, cancellationToken);
        
        if (terminal == null)
        {
            throw new Exception($"Terminal with key '{query.TerminalKey}' not found.");
        }

        var dto = new TerminalDto
        {
            Id = terminal.Id,
            Name = terminal.Name,
            TerminalKey = terminal.TerminalKey,
            Location = terminal.Location,
            FloorId = terminal.FloorId,
            HasCashDrawer = terminal.HasCashDrawer,
            OpeningBalance = terminal.OpeningBalance,
            CurrentBalance = terminal.CurrentBalance,
            AutoLogOut = terminal.AutoLogOut,
            AutoLogOutTime = terminal.AutoLogOutTime,
            ShowGuestSelection = terminal.ShowGuestSelection,
            ShowTableSelection = terminal.ShowTableSelection,
            KitchenMode = terminal.KitchenMode,
            DefaultFontSize = terminal.DefaultFontSize,
            DefaultFontFamily = terminal.DefaultFontFamily
        };

        return new GetTerminalConfigResult(dto);
    }
}
