using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services.SystemConfig;

public class UpdatePrinterMappingsCommandHandler : ICommandHandler<UpdatePrinterMappingsCommand, UpdatePrinterMappingsResult>
{
    private readonly IPrinterMappingRepository _repository;

    public UpdatePrinterMappingsCommandHandler(IPrinterMappingRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdatePrinterMappingsResult> HandleAsync(UpdatePrinterMappingsCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingMappings = await _repository.GetByTerminalIdAsync(command.TerminalId, cancellationToken);
            var existingMappingsList = existingMappings.ToList();

            // 1. Delete removed mappings for this terminal
            var commandGroupIds = command.Mappings.Select(m => m.PrinterGroupId).ToList();
            foreach (var existing in existingMappingsList)
            {
                if (!commandGroupIds.Contains(existing.PrinterGroupId))
                {
                    await _repository.DeleteAsync(existing, cancellationToken);
                }
            }

            // 2. Add or Update mappings
            foreach (var dto in command.Mappings)
            {
                var existing = existingMappingsList.FirstOrDefault(m => m.PrinterGroupId == dto.PrinterGroupId);
                if (existing != null)
                {
                    existing.Update(dto.PhysicalPrinterName);
                    await _repository.UpdateAsync(existing, cancellationToken);
                }
                else
                {
                    var newMapping = PrinterMapping.Create(command.TerminalId, dto.PrinterGroupId, dto.PhysicalPrinterName);
                    await _repository.AddAsync(newMapping, cancellationToken);
                }
            }

            return new UpdatePrinterMappingsResult(true, "Printer mappings updated successfully.");
        }
        catch (Exception ex)
        {
            return new UpdatePrinterMappingsResult(false, $"Error updating printer mappings: {ex.Message}");
        }
    }
}
