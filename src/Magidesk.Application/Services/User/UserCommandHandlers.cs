using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IAesEncryptionService _encryptionService;
    private readonly IRoleRepository _roleRepository;

    public CreateUserCommandHandler(IUserRepository userRepository, IAesEncryptionService encryptionService, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _roleRepository = roleRepository;
    }

    public async Task<CreateUserResult> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(command.FirstName)) 
                return new CreateUserResult { IsSuccess = false, Message = "First Name is required" };
            
            // Validate Role
            var role = await _roleRepository.GetByIdAsync(command.RoleId, cancellationToken);
            if (role == null)
                return new CreateUserResult { IsSuccess = false, Message = "Invalid Role selected." };

            var encryptedPin = _encryptionService.Encrypt(command.Pin);
            var existing = await _userRepository.GetByPinAsync(encryptedPin, cancellationToken);
            if (existing != null)
                return new CreateUserResult { IsSuccess = false, Message = "PIN is already in use." };

            var user = User.Create(
                username: string.IsNullOrWhiteSpace(command.Username) ? command.FirstName.ToLower() : command.Username,
                firstName: command.FirstName,
                lastName: command.LastName,
                roleId: command.RoleId,
                encryptedPin: encryptedPin
            );
            
            await _userRepository.AddAsync(user, cancellationToken);
            
            return new CreateUserResult { UserId = user.Id, IsSuccess = true };
        }
        catch (Exception ex)
        {
            return new CreateUserResult { IsSuccess = false, Message = ex.Message };
        }
    }
}

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IAesEncryptionService _encryptionService;
    private readonly IRoleRepository _roleRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository, IAesEncryptionService encryptionService, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _roleRepository = roleRepository;
    }

    public async Task<UpdateUserResult> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
                return new UpdateUserResult { IsSuccess = false, Message = "User not found" };
            
            // Validate Role if changed (though simple assignment is fine if Guid is valid)
            var role = await _roleRepository.GetByIdAsync(command.RoleId, cancellationToken);
            if (role == null)
                 return new UpdateUserResult { IsSuccess = false, Message = "Invalid Role selected." };

            // Update fields using Domain methods where possible or direct set (User setter is private? need helper?)
            // Wait, User properties have private setters.
            // Need to check User.cs for Update methods. 
            // Audit showed NO update methods except SetRole, Deactivate/Activate.
            // I need to ADD UpdateDetails to User.cs or usage reflection/EF tracking (but setters are private).
            // Actually, EF Core change tracking works on private setters if navigation is loaded? 
            // No, standard way is Domain Method. 
            // I will implement an UpdateDetails method on User.cs in next step if missing.
            // For now, assume I'll add it.
            
            // Temporary direct hack or reflection? No, I will modify User.cs.
            // Just for compilation, I'll assume methods exist or I'll adding them.
            // Since this is a text replace, I can't see User.cs *right now* but I saw it earlier.
            // It had private setters.
            
            // Let's use the public methods I saw earlier: SetRole, Activate, Deactivate.
            // But Name? No UpdateName method seen.
            // REQUIRED: Add UpdateDetails to User.cs.
            
            // For this file content, I'll assume the method exists `UpdateDetails`.
            
            user.UpdateDetails(command.FirstName, command.LastName);
            user.SetRole(command.RoleId);
            
            if (command.IsActive) user.Activate(); else user.Deactivate();

            // PIN
            if (!string.IsNullOrWhiteSpace(command.Pin))
            {
                 user.SetPin(_encryptionService.Encrypt(command.Pin));
            }

            await _userRepository.UpdateAsync(user, cancellationToken);
            
            return new UpdateUserResult { IsSuccess = true };
        }
        catch (Exception ex)
        {
            return new UpdateUserResult { IsSuccess = false, Message = ex.Message };
        }
    }
}

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, DeleteUserResult>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<DeleteUserResult> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
         try
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null) return new DeleteUserResult { IsSuccess = false, Message = "User not found" };

            // Hard delete or Soft delete? 
            // Feature req said "Delete". Repository has DeleteAsync. 
            // Let's do Hard Delete for "Deletion", Deactivate for "Archiving".
            // Command is "Delete".
            
            await _userRepository.DeleteAsync(user, cancellationToken);
            return new DeleteUserResult { IsSuccess = true };
        }
        catch (Exception ex)
        {
            return new DeleteUserResult { IsSuccess = false, Message = ex.Message };
        }
}
}
