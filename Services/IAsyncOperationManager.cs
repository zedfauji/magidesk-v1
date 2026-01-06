using System.Threading.Tasks;

namespace Magidesk.Services
{
    /// <summary>
    /// Manages async operations to prevent fire-and-forget patterns and ensure proper error handling.
    /// </summary>
    public interface IAsyncOperationManager
    {
        /// <summary>
        /// Observes an async operation and automatically handles exceptions.
        /// </summary>
        Task<T> ObserveAsync<T>(Task<T> operation, string operationName);

        /// <summary>
        /// Observes an async operation without return value and automatically handles exceptions.
        /// </summary>
        Task ObserveAsync(Task operation, string operationName);

        /// <summary>
        /// Registers a fire-and-forget operation with automatic error reporting.
        /// </summary>
        void RegisterFireAndForget(Task operation, string operationName);
    }
}