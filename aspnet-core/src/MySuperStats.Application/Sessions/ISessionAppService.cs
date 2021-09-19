using System.Threading.Tasks;
using Abp.Application.Services;
using MySuperStats.Sessions.Dto;

namespace MySuperStats.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
