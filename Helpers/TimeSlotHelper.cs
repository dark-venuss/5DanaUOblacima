using SofijaFesis_5DanaUOblacima.Models;

namespace SofijaFesis_5DanaUOblacima.Helpers
{
    public static class TimeSlotHelper
    {
        public static bool IsTimeInOperatingHours(string time, string openingTime, string closingTime)
        {
            if (!TimeOnly.TryParse(time, out var reservationTime))
                return false;
            if (!TimeOnly.TryParse(openingTime, out var opening))
                return false;
            if (!TimeOnly.TryParse(closingTime, out var closing))
                return false;

            return reservationTime >= opening && reservationTime <= closing;
        }

        public static bool DoTimeSlotsOverlap(string time1, int duration1, string time2, int duration2)
        {
            if (!TimeOnly.TryParse(time1, out var start1))
                return false;
            if (!TimeOnly.TryParse(time2, out var start2))
                return false;

            var end1 = start1.AddMinutes(duration1);
            var end2 = start2.AddMinutes(duration2);

            return start1 < end2 && start2 < end1;
        }
        public static string FormatTime(string time)
        {
            if (TimeOnly.TryParse(time, out var parsed))
                return parsed.ToString("HH:mm");
            return time;
        }
    }
}
