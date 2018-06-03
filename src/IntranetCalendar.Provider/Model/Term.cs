namespace IntranetCalendar.Provider.Model
{
    using System;

    // valueobject
    public class Term
    {
        public Term(DateTime startTime, TimeSpan duration, string reservationId)
        {
            StartTime = startTime;
            Duration = duration;
            this.ReservationId = reservationId;
        }

        public DateTime StartTime { get; }
        public TimeSpan Duration { get; }
        public string ReservationId { get; }
    }
}