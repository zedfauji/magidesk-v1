using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Services;

namespace Magidesk.Application.Commands.TableOperations;

/// <summary>
/// Handler for merging tables with billing accuracy verification.
/// </summary>
public class MergeTablesCommandHandler : ICommandHandler<MergeTablesCommand, MergeTablesResult>
{
    private readonly ITableOperationsService _tableOperationsService;

    public MergeTablesCommandHandler(ITableOperationsService tableOperationsService)
    {
        _tableOperationsService = tableOperationsService ?? throw new ArgumentNullException(nameof(tableOperationsService));
    }

    public async Task<MergeTablesResult> HandleAsync(
        MergeTablesCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.PrimaryTableId == Guid.Empty)
        {
            throw new ArgumentException("Primary table ID cannot be empty.", nameof(command));
        }

        if (command.SecondaryTableIds == null || !command.SecondaryTableIds.Any())
        {
            throw new ArgumentException("At least one secondary table ID is required.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new ArgumentException("Reason is required for table merge.", nameof(command));
        }

        if (command.StaffId == Guid.Empty)
        {
            throw new ArgumentException("Staff ID is required for authorization.", nameof(command));
        }

        var result = await _tableOperationsService.MergeTablesAsync(
            command.PrimaryTableId,
            command.SecondaryTableIds,
            command.Reason,
            command.StaffId);

        if (!result.IsSuccessful)
        {
            throw new InvalidOperationException(result.ErrorMessage ?? "Failed to merge tables");
        }

        if (result.Data == null)
        {
            throw new InvalidOperationException("Table operation result data is missing");
        }

        var allTableIds = new List<Guid> { command.PrimaryTableId };
        allTableIds.AddRange(command.SecondaryTableIds);

        return new MergeTablesResult(
            MergedTableIds: allTableIds.AsReadOnly(),
            MergedSessionId: result.Data.ResultingSessionId ?? Guid.Empty,
            TotalCharge: result.Data.TotalChargesAfter.Amount,
            TotalGuestCount: 0, // Would need to be calculated from actual sessions
            MergedAt: result.Data.OperationTimestamp,
            StaffId: result.Data.StaffId
        );
    }
}