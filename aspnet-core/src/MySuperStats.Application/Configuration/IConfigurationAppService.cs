using System.Threading.Tasks;
using MySuperStats.Configuration.Dto;

namespace MySuperStats.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
