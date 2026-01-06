using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Application.Mapping;
using MediatR;

namespace Magidesk.Application.Services;

public class CreateTableLayoutCommandHandler : IRequestHandler<CreateTableLayoutCommand, TableLayoutDto>
{
    private readonly ITableRepository _tableRepository;
    private readonly ITableLayoutRepository _tableLayoutRepository;

    public CreateTableLayoutCommandHandler(
        ITableRepository tableRepository,
        ITableLayoutRepository tableLayoutRepository)
    {
        _tableRepository = tableRepository;
        _tableLayoutRepository = tableLayoutRepository;
    }

    public async Task<TableLayoutDto> Handle(CreateTableLayoutCommand request, CancellationToken cancellationToken)
    {
        // Create the layout
        var layout = TableLayout.Create(
            request.Name,
            request.FloorId,
            request.IsDraft
        );

        // Add tables to the layout
        foreach (var tableDto in request.Tables)
        {
            var table = Table.Create(
                tableDto.TableNumber,
                tableDto.Capacity,
                tableDto.X,
                tableDto.Y,
                request.FloorId
            );
            
            layout.AddTable(table);
        }

        // Save the layout
        await _tableLayoutRepository.AddAsync(layout, cancellationToken);

        // Return the DTO
        return TableMapper.ToLayoutDto(layout);
    }
}

public class UpdateTablePositionCommandHandler : IRequestHandler<UpdateTablePositionCommand, TableDto>
{
    private readonly ITableRepository _tableRepository;

    public UpdateTablePositionCommandHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<TableDto> Handle(UpdateTablePositionCommand request, CancellationToken cancellationToken)
    {
        var table = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);
        if (table == null)
        {
            throw new KeyNotFoundException($"Table with ID {request.TableId} not found.");
        }

        // Update full geometry
        table.UpdateGeometry(request.X, request.Y, request.Shape, request.Width, request.Height);

        await _tableRepository.UpdateAsync(table, cancellationToken);

        return TableMapper.ToDto(table);
    }
}

public class AddTableToLayoutCommandHandler : IRequestHandler<AddTableToLayoutCommand, TableDto>
{
    private readonly ITableRepository _tableRepository;
    private readonly ITableLayoutRepository _tableLayoutRepository;

    public AddTableToLayoutCommandHandler(
        ITableRepository tableRepository,
        ITableLayoutRepository tableLayoutRepository)
    {
        _tableRepository = tableRepository;
        _tableLayoutRepository = tableLayoutRepository;
    }

    public async Task<TableDto> Handle(AddTableToLayoutCommand request, CancellationToken cancellationToken)
    {
        // Create new table
        var table = Table.Create(
            request.TableNumber,
            request.Capacity,
            request.X,
            request.Y,
            null, // floorId
            request.LayoutId,
            true, // isActive
            request.Shape,
            request.Width,
            request.Height
        );

        // Save table
        await _tableRepository.AddAsync(table, cancellationToken);

        return TableMapper.ToDto(table);
    }
}

public class SaveTableLayoutCommandHandler : IRequestHandler<SaveTableLayoutCommand, TableLayoutDto>
{
    private readonly ITableLayoutRepository _tableLayoutRepository;
    private readonly ITableRepository _tableRepository;

    public SaveTableLayoutCommandHandler(
        ITableLayoutRepository tableLayoutRepository,
        ITableRepository tableRepository)
    {
        _tableLayoutRepository = tableLayoutRepository;
        _tableRepository = tableRepository;
    }

    public async Task<TableLayoutDto> Handle(SaveTableLayoutCommand request, CancellationToken cancellationToken)
    {
        var layout = await _tableLayoutRepository.GetByIdAsync(request.LayoutId, cancellationToken);
        if (layout == null)
        {
            throw new KeyNotFoundException($"Layout with ID {request.LayoutId} not found.");
        }

        layout.UpdateName(request.Name);

        if (request.IsDraft.HasValue)
        {
            layout.SetDraftStatus(request.IsDraft.Value);
        }

        // Batch update table geometries? 
        // For simple recovery, we update the layout metadata. 
        // Tables are usually updated individually via UpdateTablePositionCommand in this architecture.

        await _tableLayoutRepository.UpdateAsync(layout, cancellationToken);

        return TableMapper.ToLayoutDto(layout);
    }
}

public class RemoveTableFromLayoutCommandHandler : IRequestHandler<RemoveTableFromLayoutCommand, bool>
{
    private readonly ITableLayoutRepository _tableLayoutRepository;

    public RemoveTableFromLayoutCommandHandler(ITableLayoutRepository tableLayoutRepository)
    {
        _tableLayoutRepository = tableLayoutRepository;
    }

    public async Task<bool> Handle(RemoveTableFromLayoutCommand request, CancellationToken cancellationToken)
    {
        var layout = await _tableLayoutRepository.GetByIdAsync(request.LayoutId, cancellationToken);
        if (layout == null) return false;

        layout.RemoveTable(request.TableId);
        await _tableLayoutRepository.UpdateAsync(layout, cancellationToken);

        return true;
    }
}
