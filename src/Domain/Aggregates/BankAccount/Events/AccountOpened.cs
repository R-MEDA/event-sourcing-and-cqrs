namespace Domain.Aggregates.BankAccount.Events
{
    public record AccountOpened(Guid AccountId, Guid AccountHolder, decimal InitialDeposit, string Currency = "EUR") : Event;
}

