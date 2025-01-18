namespace ChatApp.DTOs
{
    public class MessageReceived
    {
        public required Guid ChatId { get; init; }

        public required string Text { get; init; }
    }
}
