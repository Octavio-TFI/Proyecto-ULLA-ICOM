using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Embeddings
{
    internal record VectorSearchResult<TEntity> where TEntity : Entity
    {
        public required double Distance { get; init; }

        public required TEntity Entity { get; init; }
    }
}
