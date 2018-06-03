namespace IntranetCalendar.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class IntranetClient : IIntranetClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<IntranetClient> _logger;
        private static bool _loggedIn;

        private static IDictionary<string, (string page, string button)> _floorMap = new Dictionary<string, (string page, string button)>
        {
            {"Zasoby ogólne II piętro cz.1", (page: "884", button: "1533")},
            {"Zasoby ogólne II piętro cz.2", (page: "887", button: "1534")},
            {"Zasoby ogólne III piętro", (page: "888", button: "1535")},
            {"Zasoby ogólne IV piętro", (page: "889", button: "1537")},
            {"Zasoby ogólne IV pietro cz.2", (page: "1809", button: "3720")},
            {"Zasoby ogólne VI", (page: "1941", button: "3939")},
        };

        public IntranetClient(HttpClient client, ILogger<IntranetClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<string> GetCalendarBodyAsync(DateTime date, string floor)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));

            await PreLogin();

            var (page, button) = MapToPage(floor);
            _logger.LogInformation($" Floor: {floor} mapped to page: {page}");

            var parameters = new Dictionary<string, string>()
            {
                {"ScriptManager", $"ctrl_{button}$upR|ctrl_{button}$lbnChoose"},
                {"ctl09$tbxSearch", string.Empty},
                {$"ctrl_{button}$tbxDate$tbxtbxDate", $"{date:d}"},
                {$"ctrl_{button}$tbxDate$meetbxDate_ClientState", string.Empty},

                {"ScriptManager_HiddenField",string.Empty},
                {"__EVENTTARGET", $"ctrl_{button}$lbnChoose"},
                {"__EVENTARGUMENT", string.Empty},
                {"__VIEWSTATE", "/wEPZwUPOGQ1YjIxNjgwZTk0NGNiSuB1LUpW5RvB1ik2OR+KnlzLtL8="},
                {"__VIEWSTATEGENERATOR", "CA0B0334"},
                {"__ASYNCPOST", "true"}
            };

            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");

            var response = await _client.PostAsync($"/Default.aspx?PageId={page}", new FormUrlEncodedContent(parameters));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task PreLogin()
        {
            
            if (_loggedIn == false)
            {
                await _client.GetAsync("/");

                _logger.LogInformation("Authorization...");
                _loggedIn = true;
            }
        }

        private static (string page, string button) MapToPage(string floor)
        {
            if (_floorMap.ContainsKey(floor) == false)
            {
                throw new FloorDoesntExistException(floor);
            }

            return _floorMap[floor];
        }
    }
}