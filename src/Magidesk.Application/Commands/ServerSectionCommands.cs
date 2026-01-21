using MediatR;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Commands;

public record CreateServerSectionCommand(
    string Name,
    Guid ServerId,
    string Description = "",
    string Color = "#3498db"
) : IRequest<ServerSectionDto>;

public record UpdateServerSectionCommand(
    Guid SectionId,
    string Name,
    string Description = "",
    string Color = "#3498db"
) : IRequest<ServerSectionDto>;

public record DeleteServerSectionCommand(
    Guid SectionId
) : IRequest<bool>;

public record AssignTablesToServerSectionCommand(
    Guid SectionId,
    List<Guid> TableIds
) : IRequest<ServerSectionDto>;

public record RemoveTablesFromServerSectionCommand(
    Guid SectionId,
    List<Guid> TableIds
) : IRequest<ServerSectionDto>;

public record UpdateServerAssignmentCommand(
    Guid SectionId,
    Guid NewServerId
) : IRequest<ServerSectionDto>;

public record GetServerSectionsQuery(
    Guid? ServerId = null,
    bool IncludeInactive = false
) : IRequest<List<ServerSectionDto>>;

public record GetServerAssignmentsQuery(
    bool IncludeInactive = false
) : IRequest<List<ServerAssignmentDto>>;
