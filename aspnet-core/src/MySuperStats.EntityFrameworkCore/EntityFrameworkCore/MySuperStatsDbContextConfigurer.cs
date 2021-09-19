using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace MySuperStats.EntityFrameworkCore
{
    public static class MySuperStatsDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<MySuperStatsDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<MySuperStatsDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
