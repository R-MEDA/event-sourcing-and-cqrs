namespace Domain.Aggregates.BankAccount.Events
{
    public record AccountClosed(AccountId AccountId, string Reason) : Event;
}