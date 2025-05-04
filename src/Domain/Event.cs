namespace Domain
{
    public abstract record Event
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Version { get; set; }
    }
}