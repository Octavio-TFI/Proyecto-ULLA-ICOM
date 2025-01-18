namespace ChatApp.DTOs
{
    public class MessageToSend
    {
        public required Guid ChatId { get; init; }

        public required string Texto { get; init; }

        public required DateTime DateTime { get; init; }
    }
}
