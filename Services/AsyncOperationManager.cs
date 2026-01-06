using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Magidesk.Services
{
    /// <summary>
    /// Implementation of async operation manager to prevent fire-and-forget patterns.
    /// </summary>
    public class AsyncOperationManager : IAsyncOperationManager
    {
        private readonly IErrorService _errorService;
        private readonly ILogger<AsyncOperationManager> _logger;

        public AsyncOperationManager(IErrorService errorService, ILogger<AsyncOperationManager> logger)
        {
            _errorService = errorService;
            _logger = logger;
        }

        public async Task<T> ObserveAsync<T>(Task<T> operation, string operationName)
        {
            try
            {
                _logger.LogDebug("Starting async operation: {OperationName}", operationName);
                var result = await operation;
                _logger.LogDebug("Completed async operation: {OperationName}", operationName);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Async operation failed: {OperationName}", operationName);
                await _errorService.ShowErrorAsync("Operation Failed", $"{operationName} failed: {ex.Message}", ex.ToString());
                return default(T)!; // Return default value - caller should handle null appropriately
            }
        }

        public async Task ObserveAsync(Task operation, string operationName)
        {
            try
            {
                _logger.LogDebug("Starting async operation: {OperationName}", operationName);
                await operation;
                _logger.LogDebug("Completed async operation: {OperationName}", operationName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Async operation failed: {OperationName}", operationName);
                await _errorService.ShowErrorAsync("Operation Failed", $"{operationName} failed: {ex.Message}", ex.ToString());
            }
        }

        public void RegisterFireAndForget(Task operation, string operationName)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogDebug("Starting fire-and-forget operation: {OperationName}", operationName);
                    await operation;
                    _logger.LogDebug("Completed fire-and-forget operation: {OperationName}", operationName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fire-and-forget operation failed: {OperationName}", operationName);
                    _ = _errorService.ShowErrorAsync("Background Operation Failed", $"{operationName} failed: {ex.Message}", ex.ToString());
                }
            });
        }
    }
}