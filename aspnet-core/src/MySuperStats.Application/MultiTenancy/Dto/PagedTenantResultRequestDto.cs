using Abp.Application.Services.Dto;

namespace MySuperStats.MultiTenancy.Dto
{
    public class PagedTenantResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

