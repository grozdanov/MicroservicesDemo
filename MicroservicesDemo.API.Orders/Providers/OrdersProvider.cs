using AutoMapper;
using MicroservicesDemo.API.Orders.DB;
using MicroservicesDemo.API.Orders.Interfaces;
using MicroservicesDemo.API.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesDemo.API.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        private readonly OrderDbContext dbContext;
        private readonly ILogger<OrdersProvider> logger;
        private readonly IMapper mapper;

        public OrdersProvider(OrderDbContext dbContext, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            if (!dbContext.Orders.Any())
            {
                dbContext.Orders.AddRange(
                    new DB.Order() { Id = 1, CustomerId = 1, OrderDate = new DateTime(2020, 04, 01), Total = 100 },
                    new DB.Order() { Id = 2, CustomerId = 2, OrderDate = new DateTime(2020, 04, 03), Total = 200 },
                    new DB.Order() { Id = 3, CustomerId = 3, OrderDate = new DateTime(2020, 04, 04), Total = 300 },
                    new DB.Order() { Id = 4, CustomerId = 2, OrderDate = new DateTime(2020, 04, 05), Total = 200 }
                );
                dbContext.SaveChanges();
            }

            if (!dbContext.OrderItems.Any())
            {
                dbContext.OrderItems.AddRange(
                    new DB.OrderItem() { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 100 },
                    new DB.OrderItem() { Id = 2, OrderId = 2, ProductId = 2, Quantity = 3, UnitPrice = 50 },
                    new DB.OrderItem() { Id = 3, OrderId = 3, ProductId = 3, Quantity = 1, UnitPrice = 500 },
                    new DB.OrderItem() { Id = 4, OrderId = 4, ProductId = 4, Quantity = 4, UnitPrice = 80 }
                );
                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                var orders = await dbContext.Orders.Include(o => o.Items).ToListAsync();
                var ordersByCustomer = orders.Where(o => o.CustomerId == customerId);
                if (ordersByCustomer != null && ordersByCustomer.Any())
                {
                    var result = mapper.Map<IEnumerable<DB.Order>, IEnumerable<Models.Order>>(ordersByCustomer);
                    return (true, result, null);
                }
                else
                {
                    return (false, null, "Not Found");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
