using CustomerService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.DBContext
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Category>().HasData(
        //        new Category
        //        {
        //            Id = 1,
        //            Name = "Electronics",
        //            Description = "Electronic Items",
        //        },
        //        new Category
        //        {
        //            Id = 2,
        //            Name = "Clothes",
        //            Description = "Dresses",
        //        },
        //        new Category
        //        {
        //            Id = 3,
        //            Name = "Grocery",
        //            Description = "Grocery Items",
        //        }
        //    );
        //}

    }
}
