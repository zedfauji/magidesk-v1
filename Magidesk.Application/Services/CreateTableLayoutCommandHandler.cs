using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
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
            request.FloorId
        );

        // Add tables to the layout
        foreach (var tableDto in request.Tables)
        {
            var table = Table.Create(
                tableDto.TableNumber,
                tableDto.Capacity,
                (int)tableDto.X,
                (int)tableDto.Y,
                request.FloorId
            );
            
            layout.AddTable(table);
        }

        // Save the layout
        await _tableLayoutRepository.AddAsync(layout, cancellationToken);

        // Return the DTO
        return new TableLayoutDto
        {
            Id = layout.Id,
            Name = layout.Name,
            FloorId = layout.FloorId,
            Tables = layout.Tables.Select(t => new TableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                X = t.X,
                Y = t.Y,
                Status = t.Status,
                IsActive = t.IsActive
            }).ToList(),
            CreatedAt = layout.CreatedAt,
            UpdatedAt = layout.UpdatedAt,
            IsActive = layout.IsActive,
            Version = layout.Version
        };
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

        // Update position and shape
        table.UpdatePosition(request.X, request.Y);
        // Note: Shape would need to be added to Table entity in a real implementation

        await _tableRepository.UpdateAsync(table, cancellationToken);

        return new TableDto
        {
            Id = table.Id,
            TableNumber = table.TableNumber,
            Capacity = table.Capacity,
            X = table.X,
            Y = table.Y,
            Status = table.Status,
            IsActive = table.IsActive
        };
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
            request.Y
        );

        // Save table
        await _tableRepository.AddAsync(table, cancellationToken);

        return new TableDto
        {
            Id = table.Id,
            TableNumber = table.TableNumber,
            Capacity = table.Capacity,
            X = table.X,
            Y = table.Y,
            Status = table.Status,
            IsActive = table.IsActive
        };
    }
}
