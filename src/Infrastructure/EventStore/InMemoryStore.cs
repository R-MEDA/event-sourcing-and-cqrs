using System.Collections.Concurrent;
using Domain;
using Domain.Store;
using Infrastructure.EventStore.Exceptions;

namespace Infrastructure.EventStore;

public class InMemoryStore : IEventStore
{
	private readonly ConcurrentDictionary<Guid, List<Event>> _streams = new();

	public Task AppendAsync(Identity aggregateId, IEnumerable<Event> events, int expectedVersion, CancellationToken cancellationToken)
	{
		var eventList = events.ToList();
		// Skip when no new changes are provided
		if (eventList.Count == 0) return Task.CompletedTask;

		var stream = _streams.GetOrAdd(aggregateId.Id, _ => []);

		lock (stream)
		{
			// Check concurrency
			if (stream.Count != expectedVersion)
			{
				throw new ConcurrencyException($"Expected version {expectedVersion} but got {stream.Count}");
			}

			// Append events with incrementing versions
			var version = stream.Count;
			foreach (var @event in eventList)
			{
				@event.Version = ++version;
				stream.Add(@event);
			}
		}

		return Task.CompletedTask;
	}

	public Task<IEnumerable<Event>> GetEventsAsync(Identity aggregateId, CancellationToken cancellationToken)
	{
		if (_streams.TryGetValue(aggregateId.Id, out var stream))
		{
			return Task.FromResult(stream.OrderBy(e => e.Version).AsEnumerable());
		}

		return Task.FromResult<IEnumerable<Event>>([]);
	}
}