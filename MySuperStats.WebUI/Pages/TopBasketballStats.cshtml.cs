
using System;
using System.Net;
using System.Threading.Tasks;
using CustomFramework.BaseWebApi.Contracts.ApiContracts;
using CustomFramework.BaseWebApi.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySuperStats.Contracts.Enums;
using MySuperStats.Contracts.Responses;
using MySuperStats.WebUI.ApplicationSettings;
using MySuperStats.WebUI.Constants;
using MySuperStats.WebUI.Utils;
using Newtonsoft.Json;

namespace MySuperStats.WebUI.Pages
{
    public class TopBasketballStatsModel : PageModel
    {
        private readonly IWebApiConnector<WebApiResponse> _webApiConnector;
        private readonly AppSettings _appSettings;
        private readonly ISession _session;
        private readonly ILocalizationService _localizer;

        public BasketballStatisticTable StatisticTable { get; set; }

        public TopBasketballStatsModel(ISession session, IWebApiConnector<WebApiResponse> webApiConnector, AppSettings appSettings, ILocalizationService localizer)
        {
            _session = session;
            _webApiConnector = webApiConnector;
            _appSettings = appSettings;
            StatisticTable = new BasketballStatisticTable();
            _localizer = localizer;
        }

        public async Task OnGetAsync(int id, string culture)
        {
            var getUrl = $"{_appSettings.WebApiUrl}{String.Format(ApiUrls.GetMatchGroupById, id)}";
            var response = await _webApiConnector.GetAsync(getUrl, culture, SessionUtil.GetToken(_session));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var matchGroupResponse = JsonConvert.DeserializeObject<MatchGroupResponse>(response.Result.ToString());
                if (matchGroupResponse.MatchGroupType != MatchGroupType.Basketball)
                {
                    throw new ArgumentException(_localizer.GetValue("This group is just for basketball stats"));
                }
                else
                {
                    await OnGetTopStatsAsync(id, culture);
                }
            }
            else
                throw new Exception(response.Message);
        }

        public async Task OnGetTopStatsAsync(int id, string culture)
        {
            try
            {
                var getUrl = $"{_appSettings.WebApiUrl}{String.Format(ApiUrls.GetTopBasketballStats, id)}";
                var response = await _webApiConnector.GetAsync(getUrl, culture, SessionUtil.GetToken(_session));

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StatisticTable = JsonConvert.DeserializeObject<BasketballStatisticTable>(response.Result.ToString());
                }
                else
                    throw new Exception(response.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
