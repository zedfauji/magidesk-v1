using System;
using MediatR;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to set or change a category's parent (hierarchy management).
/// </summary>
public record SetCategoryParentCommand(
    Guid CategoryId,
    Guid? ParentCategoryId
) : IRequest<SetCategoryParentResult>;

public record SetCategoryParentResult(
    bool Success,
    string Message
);
