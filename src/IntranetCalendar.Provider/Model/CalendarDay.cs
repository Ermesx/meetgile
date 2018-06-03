namespace IntranetCalendar.Provider.Model
{
    using System;
    using System.Collections.Generic;

    public class CalendarDay
    {
        public CalendarDay(DateTime day, IEnumerable<RoomSchedule> rooms)
        {
            Day = day.Date;
            Rooms = rooms;
        }

        public DateTime Day { get; }

        public IEnumerable<RoomSchedule> Rooms { get; }
    }
}
