using System;

namespace Microservices.Contracts
{
    public class CatalogContracts
    {
        public record InsertedItemDto (Guid Id, string Name, string Description);

        public record UpdatedItemDto (Guid Id, string Name, string Description);

        public record DeletedItemDto (Guid Id);
    }
}
