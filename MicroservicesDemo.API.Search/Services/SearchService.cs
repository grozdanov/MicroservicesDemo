using MicroservicesDemo.API.Search.Interfaces;
using MicroservicesDemo.API.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MicroservicesDemo.API.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService ordersService;
        private readonly IProductsService productsService;
        private readonly ICustomersService customersService;

        public SearchService(IOrdersService ordersService, IProductsService productsService, ICustomersService customersService)
        {
            this.ordersService = ordersService;
            this.productsService = productsService;
            this.customersService = customersService;
        }

        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var customerResult = await customersService.GetCustomerAsync(customerId);
            var ordersResult = await ordersService.GetOrdersAsync(customerId);
            var productsResult = await productsService.GetProductsAsync();

            foreach(var order in ordersResult.Orders)
            {
                foreach(var item in order.Items)
                {
                    item.ProductName = productsResult.IsSuccess
                        ? productsResult.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name
                        : "Product information is not available";
                }
            }

            if (ordersResult.IsSuccess)
            {
                var result = new
                {
                    Customer = customerResult.IsSuccess ? customerResult.Customer : new Customer() { Name = "Customer information is not available" },
                    Orders = ordersResult.Orders
                };

                return (true, result);
            }
            return (false, "Not Found");
        }
    }
}
