using System;
using Xunit;

namespace IntranetCalendar.Provider.Test
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;

    public class CalendarProviderTests
    {
        [Fact]
        public async Task GetCalendarReturnCorrectModel()
        {
            // Arrange
            var now = DateTime.Now;
            var client = new Mock<IIntranetClient>();
            client.Setup(c => c.GetCalendarBodyAsync(It.IsAny<DateTime>(), It.IsAny<string>())).Returns(Task.FromResult(Html.Text()));

            var provider = new CalendarProvider(NullLogger<CalendarProvider>.Instance, client.Object);

            // Act 
            var calendar = await provider.GetCalendarAsync(now);

            // Assert
            Assert.Equal(now.Date, calendar.Day);
            Assert.NotEmpty(calendar.Rooms);
            var term = calendar.Rooms.First().ReservedTerms.First();
            Assert.NotEmpty(term.ReservationId);
            Assert.Equal(now.Date, term.StartTime.Date);
            Assert.True(TimeSpan.FromMinutes(0) < term.Duration);
        }
    }
}
