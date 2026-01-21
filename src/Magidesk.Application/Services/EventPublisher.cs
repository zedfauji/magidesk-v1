using Magidesk.Application.Interfaces;
using System.Threading.Tasks;

namespace Magidesk.Application.Services
{
    /// <summary>
    /// Simple in-memory event publisher for development.
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        public Task PublishAsync<T>(T @event) where T : class
        {
            // For now, just log the event
            // In a real implementation, this would publish to a message bus or event store
            System.Diagnostics.Debug.WriteLine($"Event published: {@event.GetType().Name}");
            return Task.CompletedTask;
        }
    }
}
