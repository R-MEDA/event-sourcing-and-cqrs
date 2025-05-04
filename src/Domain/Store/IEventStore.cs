namespace Domain.Store
{
    public interface IEventStore
    {
        Task AppendAsync(
            Identity aggregateId,
            IEnumerable<Event> events,
            int expectedVersion,
            CancellationToken cancellationToken
        );
        Task<IEnumerable<Event>> GetEventsAsync(
            Identity aggregateId,
            CancellationToken cancellationToken
        );
    }
}