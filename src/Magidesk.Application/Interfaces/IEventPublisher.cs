using System.Threading.Tasks;

namespace Magidesk.Application.Interfaces
{
    /// <summary>
    /// Interface for publishing domain events.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes a domain event.
        /// </summary>
        Task PublishAsync<T>(T @event) where T : class;
    }
}
