using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Commands;

public class CheckInMemberCommandHandler : IRequestHandler<CheckInMemberCommand, CheckInMemberResult>
{
    private readonly IMemberRepository _memberRepository;
    private readonly ICustomerRepository _customerRepository;

    public CheckInMemberCommandHandler(
        IMemberRepository memberRepository,
        ICustomerRepository customerRepository)
    {
        _memberRepository = memberRepository;
        _customerRepository = customerRepository;
    }

    public async Task<CheckInMemberResult> Handle(CheckInMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
            if (member == null)
            {
                return new CheckInMemberResult { IsSuccess = false, Message = "Member not found." };
            }

            if (!member.IsActive)
            {
                return new CheckInMemberResult { IsSuccess = false, Message = $"Member is not active (Status: {member.Status})." };
            }

            // Record the visit on the customer entity
            var customer = member.Customer;
            if (customer != null)
            {
                customer.RecordVisit(DateTime.UtcNow, Magidesk.Domain.ValueObjects.Money.Zero());
                await _customerRepository.UpdateAsync(customer, cancellationToken);
            }

            // In F.10, we'll eventually create a specific Visit entity if needed, 
            // but for now, we just update the customer's last visit.

            return new CheckInMemberResult 
            { 
                IsSuccess = true, 
                Message = $"Member {member.MemberNumber} checked in successfully." 
            };
        }
        catch (Exception ex)
        {
            return new CheckInMemberResult { IsSuccess = false, Message = ex.Message };
        }
    }
}
