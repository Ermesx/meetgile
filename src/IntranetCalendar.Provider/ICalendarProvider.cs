using System;

namespace IntranetCalendar.Provider
{
    using System.Threading.Tasks;
    using Model;

    public interface ICalendarProvider
    {
        Task<CalendarDay> GetCalendarAsync(DateTime day);
    }
}
