using Abp.Application.Services;
using MySuperStats.MultiTenancy.Dto;

namespace MySuperStats.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

