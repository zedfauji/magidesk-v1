using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Commands.SystemConfig;

public class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand, DeleteRoleResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public DeleteRoleCommandHandler(IRoleRepository roleRepository, IUserRepository userRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task<DeleteRoleResult> HandleAsync(DeleteRoleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(command.RoleId);
            if (role == null)
                return new DeleteRoleResult(false, "Role not found.");

            // Safety check: Prevent deleting if users are assigned to this role
            var hasUsers = await _userRepository.HasUsersInRoleAsync(command.RoleId, cancellationToken);
            if (hasUsers)
            {
                return new DeleteRoleResult(false, "Cannot delete role. It is assigned to one or more users.");
            }

           await _roleRepository.DeleteAsync(role, cancellationToken);

            return new DeleteRoleResult(true, "Role deleted successfully.");
        }
        catch (Exception ex)
        {
            return new DeleteRoleResult(false, ex.Message);
        }
    }
}
