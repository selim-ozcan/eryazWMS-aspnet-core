using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using eryaz.Authorization.Roles;
using eryaz.Authorization.Users;
using eryaz.MultiTenancy;

namespace eryaz.EntityFrameworkCore
{
    public class eryazDbContext : AbpZeroDbContext<Tenant, Role, User, eryazDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public eryazDbContext(DbContextOptions<eryazDbContext> options)
            : base(options)
        {
        }
    }
}
