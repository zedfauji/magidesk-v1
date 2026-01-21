using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public UserPermission Permissions { get; private set; }

    private Role() { }

    public static Role Create(string name, UserPermission permissions)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));

        return new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            Permissions = permissions
        };
    }

    public void UpdatePermissions(UserPermission permissions)
    {
        Permissions = permissions;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));
        Name = name;
    }
}
