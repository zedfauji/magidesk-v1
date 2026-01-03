using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using MediatR;

namespace Magidesk.Application.Services;

public class CreateServerSectionCommandHandler : IRequestHandler<CreateServerSectionCommand, ServerSectionDto>
{
    private readonly IServerSectionRepository _serverSectionRepository;
    private readonly IUserRepository _userRepository;

    public CreateServerSectionCommandHandler(
        IServerSectionRepository serverSectionRepository,
        IUserRepository userRepository)
    {
        _serverSectionRepository = serverSectionRepository;
        _userRepository = userRepository;
    }

    public async Task<ServerSectionDto> Handle(CreateServerSectionCommand request, CancellationToken cancellationToken)
    {
        // Verify server exists
        var server = await _userRepository.GetByIdAsync(request.ServerId, cancellationToken);
        if (server == null)
        {
            throw new KeyNotFoundException($"Server with ID {request.ServerId} not found.");
        }

        // Create the section
        var section = ServerSection.Create(
            request.Name,
            request.ServerId,
            request.Description,
            request.Color
        );

        await _serverSectionRepository.AddAsync(section, cancellationToken);

        return new ServerSectionDto
        {
            Id = section.Id,
            Name = section.Name,
            Description = section.Description,
            ServerId = section.ServerId,
            ServerName = $"{server.FirstName} {server.LastName}",
            TableIds = section.TableIds,
            TableCount = section.TableCount,
            Color = section.Color,
            IsActive = section.IsActive,
            CreatedAt = section.CreatedAt,
            UpdatedAt = section.UpdatedAt,
            Version = section.Version
        };
    }
}

public class AssignTablesToServerSectionCommandHandler : IRequestHandler<AssignTablesToServerSectionCommand, ServerSectionDto>
{
    private readonly IServerSectionRepository _serverSectionRepository;
    private readonly ITableRepository _tableRepository;

    public AssignTablesToServerSectionCommandHandler(
        IServerSectionRepository serverSectionRepository,
        ITableRepository tableRepository)
    {
        _serverSectionRepository = serverSectionRepository;
        _tableRepository = tableRepository;
    }

    public async Task<ServerSectionDto> Handle(AssignTablesToServerSectionCommand request, CancellationToken cancellationToken)
    {
        var section = await _serverSectionRepository.GetByIdAsync(request.SectionId, cancellationToken);
        if (section == null)
        {
            throw new KeyNotFoundException($"Server section with ID {request.SectionId} not found.");
        }

        // Verify all tables exist
        foreach (var tableId in request.TableIds)
        {
            var table = await _tableRepository.GetByIdAsync(tableId, cancellationToken);
            if (table == null)
            {
                throw new KeyNotFoundException($"Table with ID {tableId} not found.");
            }
        }

        // Add tables to section
        section.AddTables(request.TableIds);
        await _serverSectionRepository.UpdateAsync(section, cancellationToken);

        // Get server name
        var server = await _serverSectionRepository.GetServerByIdAsync(section.ServerId, cancellationToken);

        return new ServerSectionDto
        {
            Id = section.Id,
            Name = section.Name,
            Description = section.Description,
            ServerId = section.ServerId,
            ServerName = server != null ? $"{server.FirstName} {server.LastName}" : "Unknown",
            TableIds = section.TableIds,
            TableCount = section.TableCount,
            Color = section.Color,
            IsActive = section.IsActive,
            CreatedAt = section.CreatedAt,
            UpdatedAt = section.UpdatedAt,
            Version = section.Version
        };
    }
}

public class GetServerSectionsQueryHandler : IRequestHandler<GetServerSectionsQuery, List<ServerSectionDto>>
{
    private readonly IServerSectionRepository _serverSectionRepository;
    private readonly IUserRepository _userRepository;

    public GetServerSectionsQueryHandler(
        IServerSectionRepository serverSectionRepository,
        IUserRepository userRepository)
    {
        _serverSectionRepository = serverSectionRepository;
        _userRepository = userRepository;
    }

    public async Task<List<ServerSectionDto>> Handle(GetServerSectionsQuery request, CancellationToken cancellationToken)
    {
        var sections = await _serverSectionRepository.GetSectionsAsync(request.ServerId, request.IncludeInactive, cancellationToken);
        
        var result = new List<ServerSectionDto>();
        foreach (var section in sections)
        {
            var server = await _userRepository.GetByIdAsync(section.ServerId, cancellationToken);
            
            result.Add(new ServerSectionDto
            {
                Id = section.Id,
                Name = section.Name,
                Description = section.Description,
                ServerId = section.ServerId,
                ServerName = server != null ? $"{server.FirstName} {server.LastName}" : "Unknown",
                TableIds = section.TableIds,
                TableCount = section.TableCount,
                Color = section.Color,
                IsActive = section.IsActive,
                CreatedAt = section.CreatedAt,
                UpdatedAt = section.UpdatedAt,
                Version = section.Version
            });
        }

        return result;
    }
}
