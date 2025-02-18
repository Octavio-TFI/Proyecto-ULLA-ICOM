using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    internal abstract class Repository<T>(DbContext _context) where T : Entity
    {
        public async Task<T> GetAsync(int id)
        {
            return await _context.FindAsync<T>(id) ??
                throw new NotFoundException();
        }

        public async Task<T> InsertAsync(T entity)
        {
            await _context.AddAsync(entity);

            return entity;
        }

        public async Task<List<T>> InsertRangeAsync(List<T> entities)
        {
            await _context.AddRangeAsync(entities);

            return entities;
        }
    }
}
