using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class PaymentBatchRepository : IPaymentBatchRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentBatchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PaymentBatch paymentBatch)
    {
        await _context.PaymentBatches.AddAsync(paymentBatch);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PaymentBatch paymentBatch)
    {
        _context.PaymentBatches.Update(paymentBatch);
        await _context.SaveChangesAsync();
    }

    public async Task<PaymentBatch?> GetOpenBatchByTerminalIdAsync(Guid terminalId)
    {
        return await _context.PaymentBatches
            .FirstOrDefaultAsync(pb => pb.TerminalId == terminalId && pb.Status == PaymentBatchStatus.Open);
    }
    
    public async Task<PaymentBatch?> GetByIdAsync(Guid id)
    {
        return await _context.PaymentBatches.FindAsync(id);
    }
}
