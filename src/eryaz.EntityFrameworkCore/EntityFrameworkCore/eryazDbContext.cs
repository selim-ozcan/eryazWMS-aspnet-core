using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using eryaz.Authorization.Roles;
using eryaz.Authorization.Users;
using eryaz.MultiTenancy;
using eryaz.Customers;
using eryaz.Products;
using eryaz.Warehouses;
using eryaz.Documents;
using eryaz.Movements;

namespace eryaz.EntityFrameworkCore
{
    public class eryazDbContext : AbpZeroDbContext<Tenant, Role, User, eryazDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<DocumentHeader> DocumentHeaders { get; set; }
        public DbSet<DocumentDetail> DocumentDetails { get; set; }
        public DbSet<Movement> Movements { get; set; }

        public eryazDbContext(DbContextOptions<eryazDbContext> options)
            : base(options)
        {
        }
    }
}
