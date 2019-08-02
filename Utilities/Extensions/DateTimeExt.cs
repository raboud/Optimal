using System;
using System.Collections.Generic;
using System.Text;

namespace RandR.Utilities.Extensions
{
    public static class DateTimeExt
    {
        public static DateTime NextBusinessDate(this DateTime pDate)
        {
            DateTime dtNextBusinessDay = pDate;
            do
            {
                dtNextBusinessDay = dtNextBusinessDay.AddDays(1);
            } while ((dtNextBusinessDay.DayOfWeek == DayOfWeek.Saturday) || (dtNextBusinessDay.DayOfWeek == DayOfWeek.Sunday));
            return dtNextBusinessDay;
        }

        public static DateTime PreviousBusinessDate(this DateTime pDate)
        {
            DateTime dtPrevBusinessDay = pDate;
            do
            {
                dtPrevBusinessDay = dtPrevBusinessDay.AddDays(-1);
            } while ((dtPrevBusinessDay.DayOfWeek == DayOfWeek.Saturday) || (dtPrevBusinessDay.DayOfWeek == DayOfWeek.Sunday));
            return dtPrevBusinessDay;
        }
    }
}
