using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MySuperStats.MultiTenancy.Dto;

namespace MySuperStats.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
        Task<PagedResultDto<TenantWithUserPermissionsDto>> GetAllForSessionUserAsync(PagedTenantResultRequestDto input);
        Task LoginToTenant(int tenantId);
        Task RemoveOwnUserFromTenant(int tenantId);
        Task RemoveUserFromTenant(int tenantId, long userId);
    }
}

