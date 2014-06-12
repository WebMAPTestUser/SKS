using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;

namespace UpgradeHelpers.VB6.Utils
{
    /// <summary>
    /// The DateTimeHelper provides util functionality for Date and Time operations.
    /// </summary>
    public class DateTimeHelper
    {

        /// <summary>
        /// The minimal value for a Visual Basic 6 Date, which is 1899/12/30.
        /// </summary>
        private static DateTime VB6MinValue = new DateTime(1899, 12, 30);

        /// <summary>
        /// Implementation of DatePart where DateValue is an object, in some cases this function is 
        /// expected to return null which is not done by Microsoft.VisualBasic.DateAndTime.DatePart.
        /// </summary>
        /// <param name="Interval">DateInterval enumeration value or String expression representing
        /// the part of the date/time value you want to return.
        /// </param>
        /// <param name="DateValue">Date value that you want to evaluate.</param>
        /// <param name="DayOfWeek">A value chosen from the FirstDayOfWeek enumeration that specifies
        /// the first day of the week. If not specified, FirstDayOfWeek.Sunday is used.</param>
        /// <param name="WeekOfYear">A value chosen from the FirstWeekOfYear enumeration that specifies
        /// the first week of the year. If not specified, FirstWeekOfYear.Jan1 is used.</param>
        /// <returns>Returns an Integer value containing the specified component of a given Date value 
        /// or null if DateValue is null.</returns>
        public static object DatePart(string Interval, object DateValue, FirstDayOfWeek DayOfWeek, FirstWeekOfYear WeekOfYear)
        {
            if (DateValue == null)
                return null;

            if (Convert.IsDBNull(DateValue))
                return null;

            if ((DateValue is string) && (string.IsNullOrEmpty((string)DateValue)))
                return null;

            return Microsoft.VisualBasic.DateAndTime.DatePart(Interval, Convert.ToDateTime(DateValue), DayOfWeek, WeekOfYear);
        }

        /// <summary>
        /// Implementation of function Time  from Visual Basic 6. This function returns only the Time part
        /// of a System.DataTime.
        /// </summary>
        public static DateTime Time
        {
            get
            {
                return DateTime.Now.AddDays(-DateTime.Now.Date.ToOADate());
            }
        }

        /// <summary>
        /// Converts a DateTime to a String according to the format in Visual Basic 6.
        /// </summary>
        /// <param name="dateTime">The DateTime value to be converted to string.</param>
        /// <returns>The DateTime value converted to string.</returns>
        public static string ToString(DateTime dateTime)
        {
            string format = "M/d/yyyy h:mm:ss tt";
            if (dateTime.Date == VB6MinValue)
                format = "h:mm:ss tt";
            else if (dateTime.TimeOfDay.Ticks == 0)
                format = "M/d/yyyy";
            return dateTime.ToString(format);
        }
    }
}
