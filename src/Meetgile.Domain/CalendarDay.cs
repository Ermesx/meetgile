namespace Meetgile.Domain
{
    using System.Collections.Generic;
    using Colision;

    public class CalendarDay
    {
        private Dictionary<string, Meeting> _meetings;

        private ISchedule _schedule;

        public void ArrangeMetting(Meeting newMeeting) { }

        public void MoveMetting(string meetingId, Period period) { }

        public void CancelMetting(string meetingid) { }

        public void UpdateMegging(string meetingId, MeetingDetails details) { }
    }
}