using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetShiftQuery.
/// </summary>
public class GetShiftQueryHandler : IQueryHandler<GetShiftQuery, GetShiftResult>
{
    private readonly IShiftRepository _shiftRepository;

    public GetShiftQueryHandler(IShiftRepository shiftRepository)
    {
        _shiftRepository = shiftRepository;
    }

    public async Task<GetShiftResult> HandleAsync(GetShiftQuery query, CancellationToken cancellationToken = default)
    {
        var shift = await _shiftRepository.GetByIdAsync(query.ShiftId, cancellationToken);
        
        return new GetShiftResult
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

