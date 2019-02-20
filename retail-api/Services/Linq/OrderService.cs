﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Clauses;
using RetailApi.Model;
using RetailApi.ViewModels;

namespace RetailApi.Services.Linq
{
    public class OrderService : IOrderService
    {
        private readonly SomeDatabaseContext _context;

        public OrderService(SomeDatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerOrder>> GetAll()
        {
            List<CustomerOrder> orders = await (
                from o in _context.Orders.AsNoTracking()
                orderby o.OrderPlaced descending
                select new CustomerOrder
                {
                    CustomerName = $"{o.Customer.LastName}, {o.Customer.FirstName}",
                    OrderFulfilled = (o.OrderFulfilled.HasValue) ? o.OrderFulfilled.Value.ToShortDateString() : string.Empty,
                    OrderPlaced = o.OrderPlaced.ToShortDateString(),
                    OrderLineItems = (from po in o.ProductOrder
                                      select new OrderLineItem
                                      {
                                          ProductQuantity = po.Quantity,
                                          ProductName = po.Product.Name
                                      }).ToList()
                }).ToListAsync();

            return orders;
        }

        public async Task<CustomerOrder> GetById(int id)
        {
            //CustomerOrder order = await (from o in _context.Orders.AsNoTracking()
            //    where o.Id == id
            //    select new CustomerOrder
            //    {
            //        CustomerName = $"{o.Customer.LastName}, {o.Customer.FirstName}",
            //        OrderFulfilled = (o.OrderFulfilled.HasValue) ? o.OrderFulfilled.Value.ToShortDateString() : string.Empty,
            //        OrderPlaced = o.OrderPlaced.ToShortDateString(),
            //        OrderLineItems = (from po in o.ProductOrder
            //            select new OrderLineItem
            //            {
            //                ProductQuantity = po.Quantity,
            //                ProductName = po.Product.Name
            //            }).ToList()
            //    }).FirstOrDefaultAsync();
            CustomerOrder order = await (
                from o in GetOrderById(id)
                select new CustomerOrder
                {
                    CustomerName = $"{o.Customer.LastName}, {o.Customer.FirstName}",
                    OrderFulfilled = (o.OrderFulfilled.HasValue) ? o.OrderFulfilled.Value.ToShortDateString() : string.Empty,
                    OrderPlaced = o.OrderPlaced.ToShortDateString(),
                    OrderLineItems = (from po in o.ProductOrder
                                      select new OrderLineItem
                                      {
                                          ProductQuantity = po.Quantity,
                                          ProductName = po.Product.Name
                                      }).ToList()
                }).FirstOrDefaultAsync();

            return order;
        }

        public async Task<bool> Delete(int id)
        {
            bool isDeleted = false;
            Orders order = await GetOrderById(id).FirstOrDefaultAsync();

            if (order != null)
            {
                _context.RemoveRange(order.ProductOrder);
                _context.Remove(order);
                await _context.SaveChangesAsync();
                isDeleted = true;
            }

            return isDeleted;

            //bool isDeleted = false;
            //List<ProductOrder> productOrders = await (from po in _context.ProductOrder
            //    where po.OrderId == id
            //    select po).ToListAsync();

            //if (productOrders.Any())
            //{
            //    _context.RemoveRange(productOrders);
            //    //_context.Remove(order);
            //    await _context.SaveChangesAsync();
            //    isDeleted = true;
            //}

            //return isDeleted;
        }

        private IQueryable<Orders> GetOrderById(int id) =>
            from o in _context.Orders.AsNoTracking()
            where o.Id == id
            select o;
    }
}