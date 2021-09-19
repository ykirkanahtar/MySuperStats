using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using MySuperStats.MultiTenancy;

namespace MySuperStats.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}
