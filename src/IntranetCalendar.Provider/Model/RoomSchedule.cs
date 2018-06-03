namespace IntranetCalendar.Provider.Model
{
    using System.Collections.Generic;

    public class RoomSchedule
    {
        public RoomSchedule(string @for, IEnumerable<Term> reservedTerms)
        {
            For = @for;
            ReservedTerms = reservedTerms;
        }

        public string For { get; }

        public IEnumerable<Term> ReservedTerms { get; }

        public void Reserve(Reservation reservation)
        {
            // Sprawdzić czy termin nie nachodzi na pozostałe 
        }
    }

    public class Reservation
    {
    }
}