using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using MySuperStats.Configuration.Dto;

namespace MySuperStats.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : MySuperStatsAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
