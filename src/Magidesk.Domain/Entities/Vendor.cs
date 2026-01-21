using System;

namespace Magidesk.Domain.Entities;

public class Vendor
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? ContactPerson { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; }

    private Vendor() { }

    public static Vendor Create(string name, string? contactPerson = null, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Vendor name is required");

        return new Vendor
        {
            Id = Guid.NewGuid(),
            Name = name,
            ContactPerson = contactPerson,
            Email = email,
            IsActive = true
        };
    }

    public void UpdateDetails(string name, string? contact = null, string? email = null, string? phone = null, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Vendor name is required");
        Name = name;
        ContactPerson = contact;
        Email = email;
        PhoneNumber = phone;
        Address = address;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
