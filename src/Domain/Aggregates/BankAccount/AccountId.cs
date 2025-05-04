namespace Domain.Aggregates.BankAccount
{
    public record AccountId(Guid Id) : Identity(Id);
}