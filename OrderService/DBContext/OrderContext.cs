using OrderService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceCommon.Classes;

namespace OrderService.DBContext
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                        .OwnsOne(
                            p => p.OrderDetails, OrderDetails =>
                            {
                                OrderDetails.OwnsOne(o => o.OrderTotal);
                            })
                         .Property(p => p.State).HasConversion<string>(); 
        }
    }
}
