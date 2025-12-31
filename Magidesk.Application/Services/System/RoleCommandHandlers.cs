using System;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services.Administration;

public class RoleCommandHandlers : 
    ICommandHandler<CreateRoleCommand, CreateRoleResult>,
    ICommandHandler<UpdateRoleCommand, UpdateRoleResult>,
    ICommandHandler<DeleteRoleCommand, DeleteRoleResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public RoleCommandHandlers(IRoleRepository roleRepository, IUserRepository userRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task<CreateRoleResult> HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = Role.Create(command.Name, command.Permissions);
            await _roleRepository.AddAsync(role);
            return new CreateRoleResult(true, role.Id, "Role created successfully.");
        }
        catch (Exception ex)
        {
            return new CreateRoleResult(false, Guid.Empty, $"Error creating role: {ex.Message}");
        }
    }

    public async Task<UpdateRoleResult> HandleAsync(UpdateRoleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(command.RoleId);
            if (role == null)
                return new UpdateRoleResult(false, "Role not found.");

            role.SetName(command.Name);
            role.UpdatePermissions(command.Permissions);
            
            await _roleRepository.UpdateAsync(role);
            return new UpdateRoleResult(true, "Role updated successfully.");
        }
        catch (Exception ex)
        {
            return new UpdateRoleResult(false, $"Error updating role: {ex.Message}");
        }
    }

    public async Task<DeleteRoleResult> HandleAsync(DeleteRoleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(command.RoleId);
            if (role == null)
                return new DeleteRoleResult(false, "Role not found.");

            // Validation: Cannot delete if users assigned
            var users = await _userRepository.GetAllAsync();
            if (users.Any(u => u.RoleId == command.RoleId))
            {
                return new DeleteRoleResult(false, "Cannot delete role. It is assigned to one or more users.");
            }

            await _roleRepository.DeleteAsync(role);
            return new DeleteRoleResult(true, "Role deleted successfully.");
        }
        catch (Exception ex)
        {
            return new DeleteRoleResult(false, $"Error deleting role: {ex.Message}");
        }
    }
}
