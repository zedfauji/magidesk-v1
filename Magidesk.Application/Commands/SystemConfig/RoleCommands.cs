using Magidesk.Domain.Enumerations;
using System;

namespace Magidesk.Application.Commands.SystemConfig;

public record CreateRoleCommand(string Name, UserPermission Permissions);
public record CreateRoleResult(bool IsSuccess, Guid RoleId, string Message);

public record UpdateRoleCommand(Guid RoleId, string Name, UserPermission Permissions);
public record UpdateRoleResult(bool IsSuccess, string Message);

public record DeleteRoleCommand(Guid RoleId);
public record DeleteRoleResult(bool IsSuccess, string Message);
