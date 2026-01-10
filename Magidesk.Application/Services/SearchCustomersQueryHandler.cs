using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for SearchCustomersQuery.
/// </summary>
public class SearchCustomersQueryHandler : IQueryHandler<SearchCustomersQuery, IEnumerable<CustomerSearchResultDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public SearchCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerSearchResultDto>> HandleAsync(SearchCustomersQuery query, CancellationToken cancellationToken = default)
    {
        // Use repository search which already handles fuzzy matching for name, phone, email
        var customers = await _customerRepository.SearchAsync(query.SearchTerm, cancellationToken);

        // Apply pagination
        var pagedCustomers = customers
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize);

        // Map to DTO
        // Note: Membership info will be handled in a separate module F.3, 
        // for now we return default values.
        return pagedCustomers.Select(c => new CustomerSearchResultDto(
            c.Id,
            c.FullName,
            c.Phone,
            c.Email,
            false, // IsMember (placeholder)
            null,  // MembershipTier (placeholder)
            c.TotalVisits
        ));
    }
}
