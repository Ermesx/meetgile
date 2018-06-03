namespace IntranetCalendarReader.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using IntranetCalendar.Provider;
    using IntranetCalendar.Provider.Model;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class CalendarReaderProcess : IHostedService
    {
        private readonly ILogger<CalendarReaderProcess> _logger;
        private readonly ICalendarProvider _calendarProvider;

        public CalendarReaderProcess(ILogger<CalendarReaderProcess> logger, ICalendarProvider calendarProvider)
        {
            _logger = logger;
            _calendarProvider = calendarProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start calendar process...");

            // Get from arguments

            var days = new List<CalendarDay>();
            for (var i = 0; i < 31; i++)
            {
                days.Add(await _calendarProvider.GetCalendarAsync(DateTime.Parse("2018-06-01").AddDays(i)));
            }

            var lookup = days.SelectMany(x => x.Rooms)/*.Where(x => x.For.Contains("Jabłko", StringComparison.CurrentCultureIgnoreCase))*/.ToLookup(k => k.For, v => v.ReservedTerms);
            foreach (var group in lookup)
            {
                var reservations = group.SelectMany(x => x).Where(x => x.ReservationId.Contains("Gadziński"));
                if (reservations.Any())
                {
                    Console.WriteLine($"\t{group.Key}");
                    foreach (var calendarRoomReservedTerm in reservations)
                    {
                        Console.WriteLine(
                            $"\t\t{calendarRoomReservedTerm.StartTime:g} :: {calendarRoomReservedTerm.Duration:g} :: {calendarRoomReservedTerm.ReservationId}");
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop calendar process...");

            return Task.CompletedTask;
        }
    }
}