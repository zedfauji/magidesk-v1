using MediatR;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Commands;

public record ExportLayoutCommand(
    Guid LayoutId,
    string FilePath
) : IRequest<string>;

public record ImportLayoutCommand(
    string FilePath
) : IRequest<TableLayoutDto>;

public record ExportAllLayoutsCommand(
    string FilePath
) : IRequest<string>;

public record ImportLayoutsCommand(
    string FilePath,
    bool ReplaceExisting = false
) : IRequest<List<TableLayoutDto>>;

public record CreateBackupCommand(
    string FilePath
) : IRequest<string>;

public record RestoreBackupCommand(
    string FilePath,
    bool ReplaceExisting = false
) : IRequest<List<TableLayoutDto>>;

public record ValidateLayoutFileCommand(
    string FilePath
) : IRequest<LayoutValidationResult>;

public class LayoutValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public List<string> Warnings { get; set; } = new();
    public int LayoutCount { get; set; }
    public int TableCount { get; set; }
    public string Metadata { get; set; } = string.Empty;
}
