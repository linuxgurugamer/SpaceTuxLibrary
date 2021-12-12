using System;
using System.Text;
using UnityEngine;

namespace SpaceTuxUtility
{
    public static class Format_Time
    {
        public static string FormatTime(double value, int numDecimals = 0, bool hideZeroValues = false, bool expanded = false)
        {
            const double SECONDS_PER_MINUTE = 60.0;
            const double MINUTES_PER_HOUR = 60.0;
            double HOURS_PER_DAY = (GameSettings.KERBIN_TIME) ? 6.0 : 24.0;
            //double DAYS_PER_YEAR = (GameSettings.KERBIN_TIME) ? 426.0 : 365.0;

            double DAYS_PER_YEAR = Planetarium.fetch.Home.orbit.period / HOURS_PER_DAY / MINUTES_PER_HOUR/ SECONDS_PER_MINUTE;

            string sign = "";
            if (value < 0.0)
            {
                sign = "-";
                value = -value;
            }

            double seconds = value;

            long minutes = (long)(seconds / SECONDS_PER_MINUTE);
            seconds -= (long)(minutes * SECONDS_PER_MINUTE);

            long hours = (long)(minutes / MINUTES_PER_HOUR);
            minutes -= (long)(hours * MINUTES_PER_HOUR);

            long days = (long)(hours / HOURS_PER_DAY);
            hours -= (long)(days * HOURS_PER_DAY);

            long years = (long)(days / DAYS_PER_YEAR);
            days -= (long)(years * DAYS_PER_YEAR);

            StringBuilder str = new StringBuilder();

            if (sign != "")
                str.Append(sign);
            if (years > 0)
                str.Append(years.ToString("#0") + "y ");

            if (str.Length > 0 || days > 0 || (str.Length > 0 && days == 0 && !hideZeroValues))
                str.Append(days.ToString("##0") + "d ");

            if (str.Length > 0 || hours > 0 || (str.Length > 0 && hours == 0 && !hideZeroValues))
                str.Append(hours.ToString("00") + (expanded ? "h " : ":"));

            if (str.Length > 0 || minutes > 0 || (str.Length == 0 && minutes == 0 && !hideZeroValues))
                str.Append(minutes.ToString("00") + (expanded ? "m " : ":"));

            if (years == 0 && days == 0 && hours == 0 && minutes== 0)
            {
                string secondsString;
                if (numDecimals > 0)
                {
                    // ToString always rounds and we want to truncate, so format with an
                    // extra decimal place and then lop it off
                    string format = "00." + new String('0', numDecimals + 1);
                    secondsString = seconds.ToString(format);
                    secondsString = secondsString.Substring(0, secondsString.Length - 1);
                }
                else
                {
                    secondsString = Math.Floor(seconds).ToString("00");
                }
                str.Append(secondsString);
            }
            else
                str.Append(Math.Floor(seconds).ToString("00"));
            str.Append(expanded ? "s" : "");

            return str.ToString();
        }
    }
}