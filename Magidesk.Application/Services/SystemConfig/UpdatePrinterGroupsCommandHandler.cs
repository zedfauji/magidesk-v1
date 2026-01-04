using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services.SystemConfig;

public class UpdatePrinterGroupsCommandHandler : ICommandHandler<UpdatePrinterGroupsCommand, UpdatePrinterGroupsResult>
{
    private readonly IPrinterGroupRepository _repository;

    public UpdatePrinterGroupsCommandHandler(IPrinterGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdatePrinterGroupsResult> HandleAsync(UpdatePrinterGroupsCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingGroups = await _repository.GetAllAsync(cancellationToken);
            var existingGroupsList = existingGroups.ToList();

            // 1. Delete removed groups
            var commandIds = command.Groups.Select(g => g.Id).ToList();
            foreach (var existing in existingGroupsList)
            {
                if (!commandIds.Contains(existing.Id))
                {
                    await _repository.DeleteAsync(existing, cancellationToken);
                }
            }

            // 2. Add or Update groups
            foreach (var dto in command.Groups)
            {
                var existing = existingGroupsList.FirstOrDefault(g => g.Id == dto.Id);
                if (existing != null)
                {
                    existing.Update(dto.Name, dto.PrinterType);
                    existing.UpdateBehavior(
                        dto.CutBehavior,
                        dto.ShowPrices,
                        dto.RetryCount,
                        dto.RetryDelayMs,
                        dto.AllowReprint,
                        dto.FallbackPrinterGroupId);
                    await _repository.UpdateAsync(existing, cancellationToken);
                }
                else
                {
                    var newGroup = PrinterGroup.Create(dto.Name, dto.PrinterType);
                    newGroup.UpdateBehavior(
                        dto.CutBehavior,
                        dto.ShowPrices,
                        dto.RetryCount,
                        dto.RetryDelayMs,
                        dto.AllowReprint,
                        dto.FallbackPrinterGroupId);
                    await _repository.AddAsync(newGroup, cancellationToken);
                }
            }

            return new UpdatePrinterGroupsResult(true, "Printer groups updated successfully.");
        }
        catch (Exception ex)
        {
            return new UpdatePrinterGroupsResult(false, $"Error updating printer groups: {ex.Message}");
        }
    }
}
