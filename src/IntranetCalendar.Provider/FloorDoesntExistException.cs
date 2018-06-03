namespace IntranetCalendar.Provider
{
    using System;

    public class FloorDoesntExistException : Exception
    {
        public FloorDoesntExistException(string floor) : base($"There is not exist floor: {floor}")
        {
            Floor = floor;
        }

        public string Floor { get; }
    }
}