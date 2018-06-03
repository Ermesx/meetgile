namespace IntranetCalendar.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using Microsoft.Extensions.Logging;
    using Model;

    public class CalendarProvider : ICalendarProvider
    {
        private readonly ILogger<CalendarProvider> _logger;
        private readonly IIntranetClient _intranetClient;
        private readonly List<string> _floors;

        public CalendarProvider(ILogger<CalendarProvider> logger, IIntranetClient intranetClient)
        {
            _logger = logger;
            _intranetClient = intranetClient;
            _floors = new List<string>
            {
                "Zasoby ogólne II piętro cz.1",
                "Zasoby ogólne II piętro cz.2",
                "Zasoby ogólne III piętro",
                "Zasoby ogólne IV piętro",
                "Zasoby ogólne IV pietro cz.2",
                "Zasoby ogólne VI"
            };
        }

        public async Task<CalendarDay> GetCalendarAsync(DateTime date)
        {
            var tasks = _floors.Select(floor => GetRoomScheduleByFloor(date, floor));
            var result = await Task.WhenAll(tasks);

            return new CalendarDay(date, result.SelectMany(x => x));
        }

        //refactor find all div with meeting then find siblings
        private async Task<IEnumerable<RoomSchedule>> GetRoomScheduleByFloor(DateTime date, string floor)
        {
            var data = await _intranetClient.GetCalendarBodyAsync(date, floor);
            var termsTable = RetriveTermsTable(data);

            var rooms = GetRoomNames(termsTable.First());

            var terms = rooms.ToDictionary(k => k, v => new List<Term>());
            var rows = termsTable.Skip(1).ToList();

            for (var j = 0; j < rows.Count; j++)
            {
                var cells = rows[j].Descendants("td").ToList();
                var hours = j % 2 != 0
                    ? int.Parse(rows[j].PreviousSibling.PreviousSibling.Descendants("td").First().FirstChild.InnerText) + 0.5
                    : int.Parse(cells.First().FirstChild.InnerText);

                for (var i = 1; i < cells.Count; i++)
                {
                    var term = cells[i].Descendants("div").LastOrDefault();
                    if (term == null)
                    {
                        continue;
                    }

                    terms[rooms[i - 1 + j % 2]].Add(TransformTerm(term, date.Date.AddMinutes(hours * 60)));
                }
            }

            return terms.Select(x => new RoomSchedule(x.Key, x.Value));
        }

        private static Term TransformTerm(HtmlNode term, DateTime startTime)
        {
            const double hourUnit = 39;
            const int hourMinutes = 60;

            var height = int.Parse(term.Attributes["style"].Value.Split(';').First().Split(':').Last().Trim('p', 'x', ' '));
            var duration = Math.Ceiling(height / hourUnit * 2) / 2 * hourMinutes;

            var whoReserve = term.Attributes["title"].Value;

            return new Term(startTime, TimeSpan.FromMinutes(duration), whoReserve);
        }

        // refactor by select xpath
        private IList<string> GetRoomNames(HtmlNode roomsRow)
        {
            var roomNames = roomsRow.InnerText
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrWhiteSpace(x));
            return roomNames.Skip(1).ToList();
        }

        private IEnumerable<HtmlNode> RetriveTermsTable(string data)
        {
            var html = TrimUnusedData(data);

            var document = new HtmlDocument();
            document.LoadHtml(html);

            return document.DocumentNode.SelectNodes("/table[3]/tr");
        }

        private string TrimUnusedData(string data)
        {
            var startTableIndex = data.IndexOf("<table", StringComparison.Ordinal);
            var endTableIndex = data.LastIndexOf("</table>", StringComparison.Ordinal);
            return data.Substring(startTableIndex, endTableIndex - startTableIndex + "</table>".Length);
        }
    }
}