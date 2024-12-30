using Domain.Entities;
using Infrastructure.Database.Chats;
using InfrastructureTests.Database.Tests;

namespace Infrastructure.Database.Chats.Tests
{
    internal class MensajeRepositoryTests
    {
        readonly static int _chatId = 1;

        readonly static List<MensajeTexto> _mensajes =
        [new()
        {
            ChatId = _chatId,
            Texto = "1",
            DateTime = DateTime.Now,
            Tipo = TipoMensaje.Usuario
        },new()
        {
            ChatId = _chatId,
            Texto = "2",
            DateTime = DateTime.Now,
            Tipo = TipoMensaje.Asistente
        },new()
        {
            ChatId = _chatId,
            Texto = "3",
            DateTime = DateTime.Now,
            Tipo = TipoMensaje.Asistente
        },new()
        {
            ChatId = _chatId,
            Texto = "4",
            DateTime = DateTime.Now,
            Tipo = TipoMensaje.Asistente
        },new()
        {
            ChatId = _chatId,
            Texto = "5",
            DateTime = DateTime.Now,
            Tipo = TipoMensaje.Asistente
        },new()
        {
            ChatId = _chatId,
            Texto = "6",
            DateTime = DateTime.Now,
            Tipo = TipoMensaje.Asistente
        },new()
        {
            ChatId = _chatId,
            Texto = "7",
            DateTime = DateTime.Now,
            Tipo = TipoMensaje.Asistente
        },new()
        {
            ChatId = _chatId,
            Texto = "8",
            DateTime = DateTime.Now,
            Tipo = TipoMensaje.Asistente
        },new()
        {
            ChatId = _chatId,
            Texto = "9",
            DateTime = DateTime.Now,
            Tipo = TipoMensaje.Asistente
        },new()
        {
            ChatId = _chatId,
            Texto = "10",
            DateTime = DateTime.Now,
            Tipo = TipoMensaje.Asistente
        },new()
        {
            ChatId = _chatId,
            Texto = "11",
            DateTime = DateTime.Now,
            Tipo = TipoMensaje.Asistente
        }];

        [Test]
        public async Task GetUltimosMensajesChatAsync()
        {
            // Arrange
            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            await context.AddRangeAsync(_mensajes);
            await context.SaveChangesAsync();

            var repository = new MensajeRepository(context);

            // Act
            var result = await repository.GetUltimosMensajesChatAsync(_chatId);

            // Assert
            Assert.That(
                result,
                Is.EquivalentTo(_mensajes.TakeLast(10)).And.Ordered
                    .Using<MensajeTexto>(
                        (x, y) => y.DateTime.CompareTo(x.DateTime)));
        }
    }
}
