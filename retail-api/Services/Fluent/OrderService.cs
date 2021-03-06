using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailApi.Data;
using RetailApi.DataTransferObjects;
using RetailApi.Models;

namespace RetailApi.Services.Fluent
{
    public class OrderService : IOrderService
    {
        private readonly ProductsContext _context;

        public OrderService(ProductsContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerOrder>> GetAll()
        {
            List<CustomerOrder> orders = await (_context.Orders.AsNoTracking()
                .OrderByDescending(o => o.OrderPlaced)
                .Select(o => new CustomerOrder
                {
                    CustomerName = $"{o.Customer.LastName}, {o.Customer.FirstName}",
                    OrderFulfilled =
                        (o.OrderFulfilled.HasValue) ? o.OrderFulfilled.Value.ToShortDateString() : string.Empty,
                    OrderPlaced = o.OrderPlaced.ToShortDateString(),
                    OrderLineItems = (o.ProductOrder.Select(po =>
                            new OrderLineItem
                            {
                                ProductQuantity = po.Quantity,
                                ProductName = po.Product.Name
                            }))
                        .ToList()
                })).ToListAsync();

            return orders;
        }

        public async Task<CustomerOrder> GetById(int id)
        {
            CustomerOrder order = await GetOrderById(id)
                .Select(o => new CustomerOrder
                {
                    CustomerName = $"{o.Customer.LastName}, {o.Customer.FirstName}",
                    OrderFulfilled = (o.OrderFulfilled.HasValue) ? o.OrderFulfilled.Value.ToShortDateString() : string.Empty,
                    OrderPlaced = o.OrderPlaced.ToShortDateString(),
                    OrderLineItems = (o.ProductOrder.Select(po => new OrderLineItem
                                                            {
                                                                ProductQuantity = po.Quantity,
                                                                ProductName = po.Product.Name
                                                            })).ToList()
                }).FirstOrDefaultAsync();

            return order;
        }

        public async Task<bool> Delete(int id)
        {
            bool isDeleted = false;
            Order order = await GetOrderById(id)
                .Include(o => o.ProductOrder)
                .FirstOrDefaultAsync();

            if (order != null)
            {
                _context.Remove(order);
                await _context.SaveChangesAsync();
                isDeleted = true;
            }

            return isDeleted;
        }


        public async Task<Order> Create(NewOrder newOrder)
        {
            List<ProductOrder> lineItems = new List<ProductOrder>();

            foreach(var li in newOrder.OrderLineItems)
            {
                lineItems.Add(new ProductOrder
                              {
                                Quantity = li.Quantity,
                                ProductId = li.ProductId
                              });
            }

            Order order = new Order
            {
                CustomerId = newOrder.CustomerId,
                ProductOrder = lineItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        private IQueryable<Order> GetOrderById(int id) =>
            _context.Orders.AsNoTracking().Where(o => o.Id == id);
    }
}
