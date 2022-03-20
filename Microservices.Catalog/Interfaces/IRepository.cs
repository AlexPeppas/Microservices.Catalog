using Microservices.Catalog.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microservices.Catalog.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        Task DeleteAsync(Guid Id);
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid Id);
        Task InsertAsync(T Item);
        Task UpdateAsync(T Item);
    }
}