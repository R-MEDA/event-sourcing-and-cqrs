using Domain;
using Domain.Aggregates.BankAccount;
using Domain.Aggregates.BankAccount.Events;
using Infrastructure.EventStore;
using Infrastructure.EventStore.Exceptions;

namespace Infrastructure.Tests;

public class InMemoryEventStoreTest
{
	private readonly InMemoryStore _store;
	private readonly AccountId _accountId;
	private readonly CancellationToken _cancellationToken = new CancellationToken();

	public InMemoryEventStoreTest()
	{
		_store = new InMemoryStore();
		_accountId = new AccountId(Guid.CreateVersion7());
	}

	[Fact]
	public async Task AppendAsync_WhenEventsAreValid_ShouldAppendAndRetrieveEvents()
	{
		var events = new List<Event>
		{
			new AccountOpened(_accountId.Id, Guid.CreateVersion7(), 100),
			new MoneyDeposited(_accountId, 50m, "First deposit")
		};

		await _store.AppendAsync(_accountId, events, 0, _cancellationToken);
		var retrievedEvents = await _store.GetEventsAsync(_accountId, _cancellationToken);

		Assert.Equal(2, retrievedEvents.Count());
		Assert.Equal(1, retrievedEvents.First().Version);
		Assert.Equal(2, retrievedEvents.Last().Version);
	}

	[Fact]
	public async Task AppendAsync_WhenConcurrentModification_ShouldThrowConcurrencyException()
	{
		var events = new List<Event>
		{
			new AccountOpened(_accountId.Id, Guid.CreateVersion7(), 100m)
		};

		await _store.AppendAsync(_accountId, events, 0, _cancellationToken);

		// Act & Assert
		await Assert.ThrowsAsync<ConcurrencyException>(() => _store.AppendAsync(_accountId, events, 0, CancellationToken.None));
	}

	[Fact]
	public async Task GetEventsAsync_WhenStreamDoesNotExist_ShouldReturnEmpty()
	{
		var nonExistingId = new AccountId(Guid.CreateVersion7());

		var events = await _store.GetEventsAsync(nonExistingId, CancellationToken.None);

		Assert.Empty(events);
	}

	[Fact]
	public async Task AppendAsync_WhenMultipleEventBatches_ShouldMaintainVersionOrder()
	{
		var firstBatch = new List<Event>
		{
			new AccountOpened(_accountId.Id, Guid.CreateVersion7(), 100m)
		};

		var secondBatch = new List<Event>
		{
			new MoneyDeposited(_accountId, 50m, "First deposit"),
			new MoneyDeposited(_accountId, 75m, "Second deposit")
		};

		await _store.AppendAsync(_accountId, firstBatch, 0, _cancellationToken);
		await _store.AppendAsync(_accountId, secondBatch, 1, _cancellationToken);
		var allEvents = await _store.GetEventsAsync(_accountId, _cancellationToken);

		// Assert
		Assert.Equal(3, allEvents.Count());
		Assert.Equal(1, allEvents.ElementAt(0).Version);
		Assert.Equal(2, allEvents.ElementAt(1).Version);
		Assert.Equal(3, allEvents.ElementAt(2).Version);
	}
}
