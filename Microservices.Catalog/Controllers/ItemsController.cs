using Microservices.Catalog.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservices.Catalog;
using Microservices.Catalog.Entities;
using Microservices.Common.Interfaces;
using MassTransit;
using static Microservices.Contracts.CatalogContracts;

namespace Microservices.Catalog.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> repository;
        private readonly IPublishEndpoint publishEndpoint;

        private static int requestCounterSimulator = 0;

        public ItemsController(IRepository<Item> repository, IPublishEndpoint publishEndpoint)
        {
            this.repository = repository;
            this.publishEndpoint = publishEndpoint;
        }
        
        [HttpGet(Name ="getAllItems")]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {

            var items = await repository.GetAllAsync();
            
            return items.Select(it=>it.ToDtoConverter());
        }

        /*[HttpGet(Name = "getAllItemsCircuitBreaker")]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsyncCircuitBreakerSimulator()
        {
            requestCounterSimulator++;
            if (requestCounterSimulator <= 2)
            {
                Console.WriteLine($"Request {requestCounterSimulator} Delaying..");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            if (requestCounterSimulator <= 4)
            {
                Console.WriteLine($"Request {requestCounterSimulator} 500 Internal Server Error..");
                return StatusCode(500);
            }
            var items = await repository.GetAllAsync();

            return Ok(items.Select(it => it.ToDtoConverter()));
        }
        */

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

            await publishEndpoint.Publish(new InsertedItemDto(item.Id,item.Name,item.Description));

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
    
            await publishEndpoint.Publish(new UpdatedItemDto(item.Id, item.Name, item.Description));

            return NoContent();
        }

        [HttpDelete(Name ="{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var existingItem = await repository.GetByIdAsync(id);
            if (existingItem == null)
                return NoContent();

            await repository.DeleteAsync(id);

            await publishEndpoint.Publish(new DeletedItemDto(id));
            
            return NoContent();
        }
    }
}
