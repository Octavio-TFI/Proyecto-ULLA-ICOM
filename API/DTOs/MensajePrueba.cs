namespace API.DTOs
{
    public record MensajePrueba
    {
        public required Guid ChatId { get; init; }

        public required string Texto { get; init; }

        public required DateTime DateTime { get; init; }
    }
}
