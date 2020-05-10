using AutoMapper;
using MicroservicesDemo.API.Products.DB;
using MicroservicesDemo.API.Products.Profiles;
using MicroservicesDemo.API.Products.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using Xunit;

namespace MicroservicesDemo.Api.Products.Test
{
    public class ProductsServiceTest
    {
        private void CreateProducts(ProductDbContext dbContext)
        {
            dbContext.Products.AddRange(
                new Product() { Id = 1, Name = "Keyboard", Inventory = 10, Price = 80 },
                new Product() { Id = 2, Name = "Mouse", Inventory = 10, Price = 50 },
                new Product() { Id = 3, Name = "Monitor", Inventory = 10, Price = 1000 },
                new Product() { Id = 4, Name = "CPU", Inventory = 10, Price = 500 },
                new Product() { Id = 5, Name = "Speakers", Inventory = 10, Price = 250 },
                new Product() { Id = 6, Name = "SSD", Inventory = 10, Price = 400 }
            );

            dbContext.SaveChanges();
        }

        [Fact]
        public async void GetProductReturnsAllproducts()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(nameof(GetProductReturnsAllproducts))
                .Options;
            var dbContext = new ProductDbContext(options);

            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration(config => config.AddProfile(productProfile));
            var mapper = new Mapper(configuration);
            
            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var result = await productsProvider.GetProductsAsync();

            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Products);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public async void GetProductReturnsproductUsingValidId()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(nameof(GetProductReturnsproductUsingValidId))
                .Options;
            var dbContext = new ProductDbContext(options);

            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration(config => config.AddProfile(productProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var result = await productsProvider.GetProductAsync(1);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Product);
            Assert.True(result.Product.Id == 1);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public async void GetProductReturnsproductUsingInvalidId()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(nameof(GetProductReturnsproductUsingValidId))
                .Options;
            var dbContext = new ProductDbContext(options);

            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration(config => config.AddProfile(productProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var result = await productsProvider.GetProductAsync(-1);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Product);
            Assert.NotNull(result.ErrorMessage);
        }
    }
}
