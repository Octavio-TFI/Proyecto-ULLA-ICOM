namespace API.DTOs
{
    public record MensajePrueba
    {
        public required Guid ChatId { get; init; }

        public required string Mensaje { get; init; }
    }
}
