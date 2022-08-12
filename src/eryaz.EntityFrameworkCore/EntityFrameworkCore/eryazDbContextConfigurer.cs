using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace eryaz.EntityFrameworkCore
{
    public static class eryazDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<eryazDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<eryazDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
