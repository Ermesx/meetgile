namespace IntranetCalendar.Provider
{
    using System;
    using System.Threading.Tasks;

    public interface IIntranetClient
    {
        Task<string> GetCalendarBodyAsync(DateTime date, string flor);
    }
}