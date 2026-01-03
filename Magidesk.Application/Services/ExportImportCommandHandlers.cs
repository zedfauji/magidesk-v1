using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using MediatR;

namespace Magidesk.Application.Services;

public class ExportLayoutCommandHandler : IRequestHandler<ExportLayoutCommand, string>
{
    private readonly ITableLayoutRepository _tableLayoutRepository;
    private readonly TableLayoutExporter _exporter;

    public ExportLayoutCommandHandler(
        ITableLayoutRepository tableLayoutRepository,
        TableLayoutExporter exporter)
    {
        _tableLayoutRepository = tableLayoutRepository;
        _exporter = exporter;
    }

    public async Task<string> Handle(ExportLayoutCommand request, CancellationToken cancellationToken)
    {
        var layout = await _tableLayoutRepository.GetLayoutWithTablesAsync(request.LayoutId, cancellationToken);
        if (layout == null)
        {
            throw new KeyNotFoundException($"Layout with ID {request.LayoutId} not found.");
        }

        var json = _exporter.ExportToJson(layout);
        
        // Save to file
        await File.WriteAllTextAsync(request.FilePath, json, cancellationToken);
        
        return request.FilePath;
    }
}

public class ImportLayoutCommandHandler : IRequestHandler<ImportLayoutCommand, TableLayoutDto>
{
    private readonly ITableLayoutRepository _tableLayoutRepository;
    private readonly TableLayoutExporter _exporter;

    public ImportLayoutCommandHandler(
        ITableLayoutRepository tableLayoutRepository,
        TableLayoutExporter exporter)
    {
        _tableLayoutRepository = tableLayoutRepository;
        _exporter = exporter;
    }

    public async Task<TableLayoutDto> Handle(ImportLayoutCommand request, CancellationToken cancellationToken)
    {
        if (!File.Exists(request.FilePath))
        {
            throw new FileNotFoundException($"File not found: {request.FilePath}");
        }

        var json = await File.ReadAllTextAsync(request.FilePath, cancellationToken);
        var layout = _exporter.ImportFromJson(json);

        // Check if layout name already exists
        var existingLayout = await _tableLayoutRepository.GetLayoutByNameAsync(layout.Name, cancellationToken);
        if (existingLayout != null)
        {
            // Generate unique name
            var baseName = layout.Name;
            var counter = 1;
            string newName;
            do
            {
                newName = $"{baseName} (Imported {counter++})";
                existingLayout = await _tableLayoutRepository.GetLayoutByNameAsync(newName, cancellationToken);
            } while (existingLayout != null && counter < 100);
            
            layout.Name = newName;
        }

        // Create new layout
        var command = new CreateTableLayoutCommand(layout.Name, layout.FloorId, layout.Tables);
        // This would be handled by the CreateTableLayoutCommandHandler
        // For now, return the imported layout
        return layout;
    }
}

public class ExportAllLayoutsCommandHandler : IRequestHandler<ExportAllLayoutsCommand, string>
{
    private readonly ITableLayoutRepository _tableLayoutRepository;
    private readonly TableLayoutExporter _exporter;

    public ExportAllLayoutsCommandHandler(
        ITableLayoutRepository tableLayoutRepository,
        TableLayoutExporter exporter)
    {
        _tableLayoutRepository = tableLayoutRepository;
        _exporter = exporter;
    }

    public async Task<string> Handle(ExportAllLayoutsCommand request, CancellationToken cancellationToken)
    {
        var layouts = await _tableLayoutRepository.GetAllAsync(cancellationToken);
        var layoutDtos = layouts.Select(layout => new TableLayoutDto
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
        }).ToList();
        
        var json = _exporter.ExportMultipleToJson(layoutDtos);
        
        // Save to file
        await File.WriteAllTextAsync(request.FilePath, json, cancellationToken);
        
        return request.FilePath;
    }
}

public class ValidateLayoutFileCommandHandler : IRequestHandler<ValidateLayoutFileCommand, LayoutValidationResult>
{
    private readonly TableLayoutExporter _exporter;

    public ValidateLayoutFileCommandHandler(TableLayoutExporter exporter)
    {
        _exporter = exporter;
    }

    public async Task<LayoutValidationResult> Handle(ValidateLayoutFileCommand request, CancellationToken cancellationToken)
    {
        var result = new LayoutValidationResult();

        try
        {
            if (!File.Exists(request.FilePath))
            {
                result.IsValid = false;
                result.ErrorMessage = "File not found.";
                return result;
            }

            var json = await File.ReadAllTextAsync(request.FilePath, cancellationToken);
            
            // Try to parse as single layout first
            try
            {
                var layout = _exporter.ImportFromJson(json);
                result.IsValid = true;
                result.LayoutCount = 1;
                result.TableCount = layout.Tables.Count;
                result.Metadata = $"Layout: {layout.Name}";
                return result;
            }
            catch
            {
                // Try multiple layouts
                try
                {
                    var layouts = _exporter.ImportMultipleFromJson(json);
                    result.IsValid = true;
                    result.LayoutCount = layouts.Count;
                    result.TableCount = layouts.Sum(l => l.Tables.Count);
                    result.Metadata = $"Multiple layouts: {layouts.Count}";
                    return result;
                }
                catch
                {
                    // Try backup format
                    try
                    {
                        var backupLayouts = _exporter.RestoreFromBackup(json);
                        result.IsValid = true;
                        result.LayoutCount = backupLayouts.Count;
                        result.TableCount = backupLayouts.Sum(l => l.Tables.Count);
                        result.Metadata = "Backup file";
                        return result;
                    }
                    catch
                    {
                        result.IsValid = false;
                        result.ErrorMessage = "Invalid file format. Expected layout export, multiple layouts export, or backup file.";
                        return result;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.ErrorMessage = $"Validation error: {ex.Message}";
            return result;
        }
    }
}
