namespace Domain.Aggregates.Customer
{
    public class Customer
    {
        public CustomerId Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }

        public Customer(string name, string email)
        {
            Id = new CustomerId(Guid.CreateVersion7());
            Name = name;
            Email = email;
        }
    }
}

