namespace Domain.Aggregates.BankAccount.Events
{
    public record MoneyTransferred(AccountId AccountId, decimal Amount, AccountId ToAccountId, string Description) : Event;
}