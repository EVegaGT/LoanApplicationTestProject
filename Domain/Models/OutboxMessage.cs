namespace Domain.Models
{
    // This class represents an outbox message that can be used for event-driven architecture or message queuing.
    // We will use transactional outbox pattern to ensure that messages are reliably sent even in the presence of failures.
    public class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public string? Error { get; set; }
        public bool Processed { get; set; } = false;
        public int RetryCount { get; set; } = 0;
        public bool IsDeadLetter { get; set; } = false;
    }
}
