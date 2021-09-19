using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace MySuperStats.Localization
{
    public static class MySuperStatsLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(MySuperStatsConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MySuperStatsLocalizationConfigurer).GetAssembly(),
                        "MySuperStats.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
