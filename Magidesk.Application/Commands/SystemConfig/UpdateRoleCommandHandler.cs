using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Commands.SystemConfig;

public class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand, UpdateRoleResult>
{
    private readonly IRoleRepository _roleRepository;

    public UpdateRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<UpdateRoleResult> HandleAsync(UpdateRoleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(command.RoleId);
            if (role == null)
                return new UpdateRoleResult(false, "Role not found.");

            if (!string.IsNullOrWhiteSpace(command.Name))
            {
                role.SetName(command.Name);
            }

            role.UpdatePermissions(command.Permissions);

            await _roleRepository.UpdateAsync(role, cancellationToken);

            return new UpdateRoleResult(true, "Role updated successfully.");
        }
        catch (Exception ex)
        {
            return new UpdateRoleResult(false, ex.Message);
        }
    }
}
