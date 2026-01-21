using System;
using System.Threading.Tasks;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services;

public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CreateCustomerResult> HandleAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check for existing customer by phone
            var existing = await _customerRepository.GetByPhoneAsync(command.Phone);
            if (existing != null)
            {
                return new CreateCustomerResult 
                { 
                    IsSuccess = false, 
                    Message = "A customer with this phone number already exists." 
                };
            }

            var customer = Customer.Create(
                command.FirstName,
                command.LastName,
                command.Phone,
                command.Email);

            customer.UpdateContactInfo(command.Email, command.Phone, command.Address, command.City, command.PostalCode);

            await _customerRepository.AddAsync(customer);

            return new CreateCustomerResult
            {
                IsSuccess = true,
                Message = "Customer created successfully.",
                CustomerId = customer.Id
            };
        }
        catch (Exception ex)
        {
            return new CreateCustomerResult
            {
                IsSuccess = false,
                Message = $"Error creating customer: {ex.Message}"
            };
        }
    }
}

public class UpdateCustomerCommandHandler : ICommandHandler<UpdateCustomerCommand, UpdateCustomerResult>
{
    private readonly ICustomerRepository _customerRepository;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<UpdateCustomerResult> HandleAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(command.CustomerId);
            if (customer == null)
            {
                return new UpdateCustomerResult { IsSuccess = false, Message = "Customer not found." };
            }

            // Check if phone is being changed to an existing one
            if (customer.Phone != command.Phone)
            {
                var existing = await _customerRepository.GetByPhoneAsync(command.Phone);
                if (existing != null)
                {
                    return new UpdateCustomerResult 
                    { 
                        IsSuccess = false, 
                        Message = "Another customer with this phone number already exists." 
                    };
                }
            }

            customer.UpdateDetails(command.FirstName, command.LastName, null);
            customer.UpdateContactInfo(command.Email, command.Phone, command.Address, command.City, command.PostalCode);
            
            if (command.IsActive && !customer.IsActive) customer.Reactivate();
            else if (!command.IsActive && customer.IsActive) customer.Deactivate();

            await _customerRepository.UpdateAsync(customer);

            return new UpdateCustomerResult
            {
                IsSuccess = true,
                Message = "Customer updated successfully."
            };
        }
        catch (Exception ex)
        {
            return new UpdateCustomerResult
            {
                IsSuccess = false,
                Message = $"Error updating customer: {ex.Message}"
            };
        }
    }
}
