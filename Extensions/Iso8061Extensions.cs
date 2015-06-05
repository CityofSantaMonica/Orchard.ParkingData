using System;
using System.Globalization;

namespace CSM.ParkingData.Extensions
{
    public static class Iso8061Extensions
    {
        public static readonly string BasicFormat = "yyyyMMddTHHmmssZ";

        public static string ToIso8061BasicString(this DateTime datetime)
        {
            var dt = datetime;

            if (datetime.Kind != DateTimeKind.Utc)
                dt = datetime.ToUniversalTime();

            return dt.ToString(BasicFormat);
        }

        public static bool IsIso8061Basic(this string datetimeString)
        {
            DateTime parsed;
            return TryParseIso8061Basic(datetimeString, out parsed);
        }

        public static bool TryParseIso8061Basic(this string datetimeString, out DateTime parsed)
        {
            return DateTime.TryParseExact(datetimeString, BasicFormat, null, DateTimeStyles.AdjustToUniversal, out parsed);
        }
    }
}