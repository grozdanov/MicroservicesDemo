using AutoMapper;
using MicroservicesDemo.API.Products.DB;
using MicroservicesDemo.API.Products.Interfaces;
using MicroservicesDemo.API.Products.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesDemo.API.Products.Providers
{
    public class ProductsProvider : IProductsProvider
    {
        private readonly ProductDbContext dbContext;
        private readonly ILogger<ProductsProvider> logger;
        private readonly IMapper mapper;

        public ProductsProvider(ProductDbContext dbContext, ILogger<ProductsProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            if (!dbContext.Products.Any())
            {
                dbContext.Products.AddRange(
                    new DB.Product() { Id = 1, Name = "KeyBoard", Inventory = 20, Price = 100 },
                    new DB.Product() { Id = 2, Name = "Mouse", Inventory = 30, Price = 50 },
                    new DB.Product() { Id = 3, Name = "Monitor", Inventory = 10, Price = 500 },
                    new DB.Product() { Id = 4, Name = "Printer", Inventory = 5, Price = 80 }
                );

                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Product> Products, string ErrorMessage)> GetProductsAsync()
        {
            try
            {
                var products = await dbContext.Products.ToListAsync();
                if (products != null && products.Any())
                {
                    var result = mapper.Map<IEnumerable<DB.Product>, IEnumerable<Models.Product>>(products);
                    return (true, result, null);
                } else
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

        public async Task<(bool IsSuccess, Models.Product Product, string ErrorMessage)> GetProductAsync(int id)
        {
            try
            {
                var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product != null)
                {
                    var result = mapper.Map<DB.Product, Models.Product>(product);
                    return (true, result, null);
                }
                else
                {
                    return (false, null, "Not Found");
                }
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
    }
}
