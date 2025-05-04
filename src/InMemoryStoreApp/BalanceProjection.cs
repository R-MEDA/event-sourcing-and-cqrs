using Domain;
using Domain.Aggregates.BankAccount.Events;

namespace InMemoryStoreApp;

public class BalanceProjection
{
    public decimal CurrentBalance { get; private set; }
    public string? Currency { get; private set; }

    public void Apply(Event @event)
    {
        switch (@event)
        {
            case AccountOpened opened:
                CurrentBalance = opened.InitialDeposit;
                Currency = opened.Currency;
                break;
            case MoneyDeposited deposited:
                CurrentBalance += deposited.Amount;
                break;
            case MoneyWithdrawn withdrawn:
                CurrentBalance -= withdrawn.Amount;
                break;
            case MoneyTransferred transferred:
                CurrentBalance -= transferred.Amount;
                break;
        }
    }
}
