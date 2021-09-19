using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace MySuperStats.Controllers
{
    public abstract class MySuperStatsControllerBase: AbpController
    {
        protected MySuperStatsControllerBase()
        {
            LocalizationSourceName = MySuperStatsConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
