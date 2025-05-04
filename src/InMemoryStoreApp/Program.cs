using Domain;
using Domain.Aggregates.BankAccount;
using Domain.Aggregates.BankAccount.Events;
using Infrastructure.EventStore;
using InMemoryStoreApp;

var store = new InMemoryStore();
var projection = new BalanceProjection();
var cancellationToken = new CancellationToken();
try
{
	// Create and open account
	var account = new BankAccount(Guid.NewGuid(), 1000m);
	await store.AppendAsync(account.AccountId, account.Changes, account.GetVersion, cancellationToken);
	Console.WriteLine("Account opened with initial balance of 1000 EUR");

	// Get current state
	IEnumerable<Event> events = await store.GetEventsAsync(account.AccountId, cancellationToken);
	account = new BankAccount(events);

	// Make deposits
	account.Deposit(500m, "Salary deposit");
	account.Deposit(200m, "Bonus payment");
	await store.AppendAsync(account.AccountId, account.Changes, account.GetVersion, cancellationToken);
	Console.WriteLine("Made two deposits");

	// Retrieve and display all events
	var allEvents = await store.GetEventsAsync(account.AccountId, cancellationToken);
	Console.WriteLine("\nEvent stream:");

	foreach (var @event in allEvents)
	{
		projection.Apply(@event);
		Console.WriteLine($"Version {@event.Version}: {@event.GetType().Name}");
		switch (@event)
		{
			case AccountOpened opened:
				Console.WriteLine($"  Initial deposit: {opened.InitialDeposit} {opened.Currency}");
				break;
			case MoneyDeposited deposited:
				Console.WriteLine($"  Amount: {deposited.Amount}, Description: {deposited.Description}");
				break;
		}
		Console.WriteLine($"  Current Balance: {projection.CurrentBalance} {projection.Currency}");
	}

	// Try some invalid operations
	Console.WriteLine("\nTrying to close account with non-zero balance...");
	account.Close("Trying to close");
}
catch (Exception ex)
{
	Console.WriteLine($"\nError: {ex.Message}");
}
