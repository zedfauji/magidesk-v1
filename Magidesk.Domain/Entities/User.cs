using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? EncryptedPin { get; private set; } // Hashed PIN
    public string? EncryptedPassword { get; private set; } // Hashed Password
    public Guid RoleId { get; private set; }
    public Money HourlyRate { get; private set; }
    public bool IsActive { get; private set; }

    // Private constructor for EF Core
    private User() 
    {
        HourlyRate = Money.Zero();
    }

    public static User Create(
        string username,
        string firstName,
        string lastName,
        Guid roleId,
        string? encryptedPin = null,
        string? encryptedPassword = null,
        decimal? hourlyRate = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            RoleId = roleId,
            EncryptedPin = encryptedPin,
            EncryptedPassword = encryptedPassword,
            HourlyRate = hourlyRate.HasValue ? new Money(hourlyRate.Value) : Money.Zero(),
            IsActive = true
        };
    }
    
    public void SetRole(Guid roleId)
    {
        RoleId = roleId;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
