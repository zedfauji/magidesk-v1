using System;

namespace Magidesk.Application.DTOs;

/// <summary>
/// Data transfer object for customer search results.
/// </summary>
public record CustomerSearchResultDto(
    Guid Id,
    string FullName,
    string Phone,
    string? Email,
    bool IsMember,
    string? MembershipTier,
    int TotalVisits
);
