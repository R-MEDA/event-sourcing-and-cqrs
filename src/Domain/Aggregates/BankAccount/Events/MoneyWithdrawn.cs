namespace Domain.Aggregates.BankAccount.Events
{
    public record MoneyWithdrawn (AccountId AccountId, decimal Amount) : Event;
}