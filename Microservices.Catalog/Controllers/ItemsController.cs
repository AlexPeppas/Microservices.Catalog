using Microservices.Catalog.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservices.Catalog;
using Microservices.Catalog.Entities;
using Microservices.Common.Interfaces;

namespace Microservices.Catalog.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> repository;

        public ItemsController(IRepository<Item> repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {

            var items = await repository.GetAllAsync();
            
            return items.Select(it=>it.ToDtoConverter());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
                return NoContent();
            else
                return item.ToDtoConverter();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto request)
        {
            var item = new Item
            {
                Id= Guid.NewGuid(), 
                Name= request.Name,
                Description = request.Description, 
                Price = request.Price,
                CreatedDate = DateTimeOffset.UtcNow 
            };

            await repository.InsertAsync(item);

            return CreatedAtAction(nameof(GetByIdAsync), new { Id = item.Id }, item);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto request)
        {
            var existingItem = await repository.GetByIdAsync(id);
            if (existingItem == null)
                return NoContent();

            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.UpdateAsync(item);
    
            return NoContent();
        }

        [HttpDelete(Name ="{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var existingItem = await repository.GetByIdAsync(id);
            if (existingItem == null)
                return NoContent();

            await repository.DeleteAsync(id);

            return NoContent();
        }
    }
}
