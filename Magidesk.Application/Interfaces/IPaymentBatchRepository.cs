using System;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IPaymentBatchRepository
{
    Task AddAsync(PaymentBatch paymentBatch);
    Task UpdateAsync(PaymentBatch paymentBatch);
    Task<PaymentBatch?> GetOpenBatchByTerminalIdAsync(Guid terminalId);
    Task<PaymentBatch?> GetByIdAsync(Guid id);
}
