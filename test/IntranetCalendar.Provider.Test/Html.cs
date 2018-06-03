namespace IntranetCalendar.Provider.Test
{
    using System;
    using System.IO;

    public class Html
    {
        private static string _html;
        public static string Text()
        {
            return _html ?? (_html = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\calendar.txt"));
        }
    }
}