using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MySuperStats.EntityFrameworkCore;
using MySuperStats.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace MySuperStats.Web.Tests
{
    [DependsOn(
        typeof(MySuperStatsWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class MySuperStatsWebTestModule : AbpModule
    {
        public MySuperStatsWebTestModule(MySuperStatsEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MySuperStatsWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(MySuperStatsWebMvcModule).Assembly);
        }
    }
}