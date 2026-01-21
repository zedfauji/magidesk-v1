using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Commands.SystemConfig;

public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, CreateRoleResult>
{
    private readonly IRoleRepository _roleRepository;

    public CreateRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<CreateRoleResult> HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(command.Name))
                return new CreateRoleResult(false, Guid.Empty, "Role name cannot be empty.");

            var existing = await _roleRepository.GetByNameAsync(command.Name, cancellationToken);
            if (existing != null)
                return new CreateRoleResult(false, Guid.Empty, $"A role with the name '{command.Name}' already exists.");

            var role = Role.Create(command.Name, command.Permissions);

            await _roleRepository.AddAsync(role, cancellationToken);

            return new CreateRoleResult(true, role.Id, "Role created successfully.");
        }
        catch (Exception ex)
        {
            return new CreateRoleResult(false, Guid.Empty, ex.Message);
        }
    }
}
