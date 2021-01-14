using Microsoft.EntityFrameworkCore;
using ShoppingSystem.Models;

namespace ShoppingSystem.Data
{
    public class ShoppingContext : DbContext
    {
        public ShoppingContext(DbContextOptions<ShoppingContext> options): base(options){}

        public ShoppingContext()
        {
            
        }
        
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Supermarket> Supermarkets { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetails> OrdersDetails { get; set; }
    }
}