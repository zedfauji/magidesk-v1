using System;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

public class MerchantBatchService : IMerchantBatchService
{
    private readonly IPaymentBatchRepository _paymentBatchRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;

    public MerchantBatchService(
        IPaymentBatchRepository paymentBatchRepository,
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway)
    {
        _paymentBatchRepository = paymentBatchRepository;
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
    }

    public async Task<PaymentBatch> OpenBatchAsync(Guid terminalId)
    {
        var activeBatch = await _paymentBatchRepository.GetOpenBatchByTerminalIdAsync(terminalId);
        if (activeBatch != null)
        {
            return activeBatch;
        }

        var newBatch = new PaymentBatch(terminalId);
        await _paymentBatchRepository.AddAsync(newBatch);
        return newBatch;
    }

    public async Task<PaymentBatch> CloseBatchAsync(Guid terminalId, UserId closedBy)
    {
        // 1. Get Open Batch
        var batch = await _paymentBatchRepository.GetOpenBatchByTerminalIdAsync(terminalId);
        if (batch == null)
        {
            // If no open batch, create one just to close it (so we have a record of the settlement)
            batch = new PaymentBatch(terminalId);
            await _paymentBatchRepository.AddAsync(batch);
        }

        // 2. Identify Payments to Settle
        var paymentsDetails = await _paymentRepository.GetUnbatchedCapturedPaymentsAsync(terminalId);
        
        // 3. Call Gateway
        // Note: Real gateways might require the LIST of transactions. Mock/Simple gateways just Close Current Batch.
        var gatewayResult = await _paymentGateway.CloseBatchAsync(terminalId);

        if (!gatewayResult.Success)
        {
            throw new Exception($"Gateway Batch Close Failed: {gatewayResult.ErrorMessage}");
        }

        // 4. Update Batch Record
        batch.Close(gatewayResult.GatewayBatchId);
        await _paymentBatchRepository.UpdateAsync(batch);

        // 5. Update Payments
        foreach (var payment in paymentsDetails)
        {
            payment.SetBatchId(batch.Id);
            // We should also possibly set status to "Settled" if we had that status.
            // For now, BatchId presence implies it's in a closed batch (Settled).
            await _paymentRepository.UpdateAsync(payment);
        }

        return batch;
    }
}
