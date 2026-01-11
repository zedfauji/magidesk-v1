using System;
using System.Collections.Generic;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a customer/patron of the club.
/// </summary>
public class Customer
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? Email { get; private set; }
    public string Phone { get; private set; } = string.Empty;
    public DateTime? DateOfBirth { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? PostalCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastVisitAt { get; private set; }
    public int TotalVisits { get; private set; }
    public Money TotalSpent { get; private set; }
    public bool IsActive { get; private set; }

    // Private constructor for EF Core
    private Customer()
    {
        TotalSpent = Money.Zero();
    }

    /// <summary>
    /// Creates a new customer record.
    /// </summary>
    public static Customer Create(string firstName, string lastName, string phone, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new BusinessRuleViolationException("First name is required.");
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new BusinessRuleViolationException("Last name is required.");

        if (string.IsNullOrWhiteSpace(phone))
            throw new BusinessRuleViolationException("Phone number is required.");

        // Simple validation for phone format (E.164-ish or basic digits)
        if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\+?[1-9]\d{1,14}$"))
            throw new BusinessRuleViolationException("Invalid phone number format.");

        if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
            throw new BusinessRuleViolationException("Invalid email format.");

        return new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            TotalVisits = 0,
            TotalSpent = Money.Zero(),
            IsActive = true
        };
    }

    /// <summary>
    /// Updates the customer's contact information.
    /// </summary>
    public void UpdateContactInfo(string? email, string phone, string? address, string? city = null, string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new BusinessRuleViolationException("Phone number is required.");

        if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\+?[1-9]\d{1,14}$"))
            throw new BusinessRuleViolationException("Invalid phone number format.");

        if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
            throw new BusinessRuleViolationException("Invalid email format.");

        Phone = phone;
        Email = email;
        Address = address;
        City = city;
        PostalCode = postalCode;
    }

    /// <summary>
    /// Updates personal details.
    /// </summary>
    public void UpdateDetails(string firstName, string lastName, DateTime? dob)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new BusinessRuleViolationException("First name is required.");
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new BusinessRuleViolationException("Last name is required.");

        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dob;
    }

    /// <summary>
    /// Records a customer visit and spending.
    /// </summary>
    public void RecordVisit(DateTime visitTime, Money spent)
    {
        if (spent < Money.Zero())
            throw new BusinessRuleViolationException("Spent amount cannot be negative.");

        TotalVisits++;
        TotalSpent += spent;
        LastVisitAt = visitTime;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }
}
