using Microservices.Catalog.Dtos;
using Microservices.Catalog.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservices.Catalog
{
    public static class DtoExtensions
    {
        public static ItemDto ToDtoConverter(this Item Item)
        {
            return new ItemDto(Item.Id, Item.Name, Item.Description, Item.Price, Item.CreatedDate);
        }
    }
}
