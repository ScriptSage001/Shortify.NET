namespace Shortify.NET.Common.Messaging.Outbox
{
    public class OutboxMessageConsumer
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
