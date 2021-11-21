using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Domain.Uow;
using MySuperStats.Sessions.Dto;

namespace MySuperStats.Sessions
{
    public class SessionAppService : MySuperStatsAppServiceBase, ISessionAppService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public SessionAppService(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            int? userTenantId = null;
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant,
                AbpDataFilters.MustHaveTenant))
            {
                if (AbpSession.UserId.HasValue)
                {
                    output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
                    userTenantId = output.User.TenantId > 0 ? output.User.TenantId : null;
                    AbpSession.Use(userTenantId, output.User.Id);
                }
            }

            if (AbpSession.TenantId.HasValue)
            {
                using (_unitOfWorkManager.Current.SetTenantId(userTenantId))
                {
                    output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
                }
            }

            return output;
        }
    }
}