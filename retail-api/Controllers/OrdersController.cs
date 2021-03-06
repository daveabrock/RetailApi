﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RetailApi.DataTransferObjects;
using RetailApi.Models;
using RetailApi.Services;

namespace RetailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerOrder>>> Get() => 
            await _orderService.GetAll();

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerOrder>> GetById(int id) =>
            await _orderService.GetById(id);

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool isDeleted = await _orderService.Delete(id);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<CustomerOrder>> Create(NewOrder newOrder)
        {
            // Create the order
            Order order = await _orderService.Create(newOrder);

            return CreatedAtAction(
                nameof(GetById), new { id = order.Id }, order);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(long id, Product product)
        //{
        //    if (id != product.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(product).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
    }
}