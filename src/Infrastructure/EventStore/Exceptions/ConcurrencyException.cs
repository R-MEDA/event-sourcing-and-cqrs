namespace Infrastructure.EventStore.Exceptions
{
    public class ConcurrencyException(string message) : Exception(message) { }
}