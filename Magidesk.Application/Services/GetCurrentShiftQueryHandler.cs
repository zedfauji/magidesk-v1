using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetCurrentShiftQuery.
/// </summary>
public class GetCurrentShiftQueryHandler : IQueryHandler<GetCurrentShiftQuery, GetCurrentShiftResult>
{
    private readonly IShiftRepository _shiftRepository;

    public GetCurrentShiftQueryHandler(IShiftRepository shiftRepository)
    {
        _shiftRepository = shiftRepository;
    }

    public async Task<GetCurrentShiftResult> HandleAsync(GetCurrentShiftQuery query, CancellationToken cancellationToken = default)
    {
        var shift = await _shiftRepository.GetCurrentShiftAsync(cancellationToken);
        
        return new GetCurrentShiftResult
        {
            Shift = shift != null ? MapToDto(shift) : null
        };
    }

    private static ShiftDto MapToDto(Domain.Entities.Shift shift)
    {
        return new ShiftDto
        {
            Id = shift.Id,
            Name = shift.Name,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            IsActive = shift.IsActive
        };
    }
}

