using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MySuperStats.Configuration;

namespace MySuperStats.Web.Host.Startup
{
    [DependsOn(
       typeof(MySuperStatsWebCoreModule))]
    public class MySuperStatsWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public MySuperStatsWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MySuperStatsWebHostModule).GetAssembly());
        }
    }
}
