using System;

namespace Microservices.Catalog.Entities
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}