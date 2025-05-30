using System;
using System.Globalization;


namespace Tazkartk.Application.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToEgyptTimeString(this DateTime utcDate)
        {
            var arabicCulture = new CultureInfo("ar-EG");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            arabicCulture.DateTimeFormat.AMDesignator = "صباحا";
            arabicCulture.DateTimeFormat.PMDesignator = "مساء";

            var egyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            var egyTime = TimeZoneInfo.ConvertTimeFromUtc(utcDate, egyTimeZone);
            return egyTime.ToString("dddd yyyy-MM-dd HH:mm tt", arabicCulture);
        }
        public static string ToArabicDateString(this DateOnly date)
        {
            var arabicCulture = new CultureInfo("ar-EG");
            return date.ToString("yyyy-MM-dd", arabicCulture);
        }

        public static string ToArabicDayString(this DateOnly date)
        {
            var arabicCulture = new CultureInfo("ar-EG");
            return date.ToString("dddd", arabicCulture);
        }

        public static string ToArabicTimeString(this TimeOnly time)
        {
            var arabicCulture = new CultureInfo("ar-EG");
            arabicCulture.DateTimeFormat.AMDesignator = "صباحا";
            arabicCulture.DateTimeFormat.PMDesignator = "مساء";
            return time.ToString("hh:mm tt", arabicCulture);
        }
    }
}
