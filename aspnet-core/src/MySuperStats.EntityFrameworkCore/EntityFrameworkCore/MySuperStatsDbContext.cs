using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using MySuperStats.Authorization.Roles;
using MySuperStats.Authorization.Users;
using MySuperStats.MultiTenancy;

namespace MySuperStats.EntityFrameworkCore
{
    public class MySuperStatsDbContext : AbpZeroDbContext<Tenant, Role, User, MySuperStatsDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public MySuperStatsDbContext(DbContextOptions<MySuperStatsDbContext> options)
            : base(options)
        {
        }
    }
}
