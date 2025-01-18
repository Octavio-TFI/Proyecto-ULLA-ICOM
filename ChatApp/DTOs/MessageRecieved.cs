namespace ChatApp.DTOs
{
    public class MessageRecieved
    {
        public required Guid ChatId { get; init; }

        public required string Text { get; init; }
    }
}
