using Domain.Entities.ConsultaAgregado;
using InfrastructureTests.Database.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Repositories.Tests
{
    internal class ConsultaRepositoryTests
    {
        [Test]
        public async Task GetAllIdsAsync()
        {
            // Arrange
            var consultas = new List<Consulta>
            {
                new()
                {
                    RemoteId = 1,
                    Titulo = "Titulo0",
                    EmbeddingTitulo = [0.1f, 0.2f, 0.3f],
                    Descripcion = "Descripcion0",
                    EmbeddingDescripcion = [1, 1, 1],
                    Solucion = "Solucion0"
                },
                new()
                {
                    RemoteId = 2,
                    Titulo = "Titulo1",
                    EmbeddingTitulo = [0.4f, 0.5f, 0.6f],
                    Descripcion = "Descripcion1",
                    EmbeddingDescripcion = [1, 1, 1],
                    Solucion = "Solucion1"
                }
            };

            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            await context.Consultas.AddRangeAsync(consultas);
            await context.SaveChangesAsync();

            var repository = new ConsultaRepository(context);

            // Act
            var result = await repository.GetAllIdsAsync();

            // Assert
            Assert.That(result, Is.EquivalentTo(new int[] { 1, 2 }));
        }

        [Test]
        public async Task GetConsultasSimilaresAsync()
        {
            // Arrange
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };

            var consultas = new List<Consulta>
            {
                new()
                {
                    RemoteId = 1,
                    Titulo = "Titulo0",
                    EmbeddingTitulo = [0.1f, 0.2f, 0.3f],
                    Descripcion = "Descripcion0",
                    EmbeddingDescripcion = [1, 1, 1],
                    Solucion = "Solucion0"
                },
                new()
                {
                    RemoteId = 2,
                    Titulo = "Titulo1",
                    EmbeddingTitulo = [0.4f, 0.5f, 0.6f],
                    Descripcion = "Descripcion1",
                    EmbeddingDescripcion = [1, 1, 1],
                    Solucion = "Solucion1"
                },
                new()
                {
                    RemoteId = 3,
                    Titulo = "Titulo2",
                    EmbeddingTitulo = [0.7f, 0.8f, 0.9f],
                    Descripcion = "Descripcion2",
                    EmbeddingDescripcion = [1, 1, 1],
                    Solucion = "Solucion2"
                },
                new()
                {
                    RemoteId = 4,
                    Titulo = "Titulo3",
                    EmbeddingTitulo = [-0.7f, -0.8f, -0.9f],
                    Descripcion = "Descripcion3",
                    EmbeddingDescripcion = [-1,-1,-1],
                    Solucion = "Solucion3"
                },
                new()
                {
                    RemoteId = 5,
                    Titulo = "Titulo4",
                    EmbeddingTitulo = [0.7f, 0.8f, 0.9f],
                    Descripcion = "Descripcion4",
                    EmbeddingDescripcion = [1, 1, 1],
                    Solucion = "Solucion4"
                },
                new()
                {
                    RemoteId = 6,
                    Titulo = "Titulo5",
                    EmbeddingTitulo = [0.7f, 0.8f, 0.9f],
                    Descripcion = "Descripcion5",
                    EmbeddingDescripcion = [1, 1, 1],
                    Solucion = "Solucion5"
                },
            };

            var context = DatabaseTestsHelper.CreateInMemoryChatContext();
            await context.Consultas.AddRangeAsync(consultas);
            await context.SaveChangesAsync();

            var repository = new ConsultaRepository(context);

            // Act
            var result = await repository.GetConsultasSimilaresAsync(embedding);

            // Assert
            Assert.That(result, Has.Count.EqualTo(5));
            Assert.That(result, Has.No.Member(consultas[3]));
        }
    }
}
