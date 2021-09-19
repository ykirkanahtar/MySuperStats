using System.Threading.Tasks;
using Abp.Application.Services;
using MySuperStats.Authorization.Accounts.Dto;

namespace MySuperStats.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
