using Abp.AutoMapper;
using Abp.MailKit;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MySuperStats.Authorization;

namespace MySuperStats
{
    [DependsOn(
        typeof(MySuperStatsCoreModule), 
        typeof(AbpAutoMapperModule),
        typeof(AbpMailKitModule))]
    public class MySuperStatsApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<MySuperStatsAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(MySuperStatsApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
