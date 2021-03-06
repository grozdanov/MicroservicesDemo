﻿using AutoMapper;
using MicroservicesDemo.API.Customers.DB;
using MicroservicesDemo.API.Customers.Interfaces;
using MicroservicesDemo.API.Customers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MicroservicesDemo.API.Customers.Providers
{
    public class CustomersProvider : ICustomersProvider
    {
        private readonly CustomerDbContext dbContext;
        private readonly ILogger<CustomersProvider> logger;
        private readonly IMapper mapper;

        public CustomersProvider(CustomerDbContext dbContext, ILogger<CustomersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            if (!dbContext.Customers.Any())
            {
                dbContext.Customers.AddRange(
                    new DB.Customer() { Id = 1, Name = "Jessica Chastain", Address = "California, USA" },
                    new DB.Customer() { Id = 2, Name = "Idrissa Elba", Address = "London, UK" },
                    new DB.Customer() { Id = 3, Name = "Aaron Sorkin", Address = "New York, USA" },
                    new DB.Customer() { Id = 4, Name = "Kevin Costner", Address = "California, USA" }
                );

                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Customer> Customers, string ErrorMessage)> GetCustomersAsync()
        {
            try
            {
                var customers = await dbContext.Customers.ToListAsync();
                if (customers != null && customers.Any())
                {
                    var result = mapper.Map<IEnumerable<DB.Customer>, IEnumerable<Models.Customer>>(customers);
                    return (true, result, null);
                } else {
                    return (false, null, "Not Found");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, Models.Customer Customer, string ErrorMessage)> GetCustomerAsync(int id)
        {
            try
            {
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
                if (customer != null)
                {
                    var result = mapper.Map<DB.Customer, Models.Customer>(customer);
                    return (true, result, null);
                }
                else
                {
                    return (false, null, "Not Found");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
