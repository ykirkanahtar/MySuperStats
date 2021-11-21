using System.Collections.Generic;
using Abp.Configuration;

namespace MySuperStats.Configuration
{
    public class SmtpSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(Abp.Net.Mail.EmailSettingNames.DefaultFromAddress, "info@mysuperstats.com"),
                new SettingDefinition(Abp.Net.Mail.EmailSettingNames.DefaultFromDisplayName, "MySuperStats.com"),
                new SettingDefinition(Abp.Net.Mail.EmailSettingNames.Smtp.Host, "srvw52.hostixo.com"),
                new SettingDefinition(Abp.Net.Mail.EmailSettingNames.Smtp.Port, "465"),
                new SettingDefinition(Abp.Net.Mail.EmailSettingNames.Smtp.UserName, "info@mysuperstats.com"),
                new SettingDefinition(Abp.Net.Mail.EmailSettingNames.Smtp.Password, "W05d_8fp"),
                new SettingDefinition(Abp.Net.Mail.EmailSettingNames.Smtp.EnableSsl, "true"),
                new SettingDefinition(Abp.Net.Mail.EmailSettingNames.Smtp.UseDefaultCredentials, "false"),
            };
        }
    }
}