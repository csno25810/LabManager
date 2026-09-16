using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Ical.Net.DataTypes;
using IcalCalendar = Ical.Net.Calendar;

namespace LabManager
{
    public class CalendarEventItem
    {
        public DateTime Start { get; set; }
        public string Summary { get; set; }
    }

    /// <summary>
    /// Google カレンダー ICS の取得・解析（Form10 / Form14 共通）
    /// </summary>
    public static class CalendarHelper
    {
        public static string DownloadIcs(CalendarSetting settings)
        {
            string icsUrl = settings.GetIcsUrl();
            if (string.IsNullOrWhiteSpace(icsUrl))
                return null;

            using (var client = new WebClient { Encoding = Encoding.UTF8 })
            {
                return client.DownloadString(icsUrl);
            }
        }

        public static List<DateTime> GetEventDatesInMonth(string icsData, DateTime targetMonth)
        {
            var dates = new List<DateTime>();
            if (string.IsNullOrWhiteSpace(icsData))
                return dates;

            var calendar = IcalCalendar.Load(icsData);
            var monthStart = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            var monthEnd = monthStart.AddMonths(1);
            var searchStart = new CalDateTime(monthStart);

            foreach (var evt in calendar.Events)
            {
                foreach (var occurrence in evt.GetOccurrences(searchStart))
                {
                    var start = occurrence.Period.StartTime;
                    if (start == null)
                        continue;

                    var date = start.Value.Date;
                    if (date >= monthEnd)
                        break;
                    if (date >= monthStart)
                        dates.Add(date);
                }
            }

            return dates.Distinct().ToList();
        }

        public static List<CalendarEventItem> GetEventsOnDate(string icsData, DateTime date)
        {
            var items = new List<CalendarEventItem>();
            if (string.IsNullOrWhiteSpace(icsData))
                return items;

            var calendar = IcalCalendar.Load(icsData);
            var targetDate = date.Date;
            var searchStart = new CalDateTime(targetDate);

            foreach (var evt in calendar.Events)
            {
                foreach (var occurrence in evt.GetOccurrences(searchStart))
                {
                    var start = occurrence.Period.StartTime;
                    if (start == null)
                        continue;

                    if (start.Value.Date != targetDate)
                        continue;

                    items.Add(new CalendarEventItem
                    {
                        Start = start.Value,
                        Summary = string.IsNullOrWhiteSpace(evt.Summary) ? "(予定)" : evt.Summary.Trim()
                    });
                }
            }

            return items.OrderBy(x => x.Start).ToList();
        }
    }
}
