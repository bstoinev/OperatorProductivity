using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace Powerfront.BackendTest.Models
{
    public class OperatorReportViewModelRef
    {
        // TODO: Use MemoryCache or more advanced caching mechanism.
        private static List<string> DeviceCache = new List<string>();
        private static List<string> WebsiteCache = new List<string>();

        static OperatorReportViewModelRef()
        {
            var cs = ConfigurationManager.ConnectionStrings["chat"].ConnectionString;

            using (var conn = new SqlConnection(cs))
            using (var cmdDevice = conn.CreateCommand())
            using (var cmdWebsite = conn.CreateCommand())
            {
                cmdDevice.CommandText = "SELECT DISTINCT Device FROM [Visitor] WHERE Device IS NOT NULL";
                cmdWebsite.CommandText = "SELECT DISTINCT Website FROM [Conversation] WHERE Website IS NOT NULL";

                conn.Open();

                var dr = cmdDevice.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        DeviceCache.Add(dr.GetString(0));
                    }
                }

                dr.Close();

                dr = cmdWebsite.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        WebsiteCache.Add(dr.GetString(0));
                    }
                }
            }
        }

        public OperatorReportViewModelRef()
        {
            var unixEpoch = new DateTime(1970, 1, 1);

            var weekStart = DateTime.Today.AddDays(
              (int)Thread.CurrentThread.CurrentUICulture.DateTimeFormat.FirstDayOfWeek - (int)DateTime.Today.DayOfWeek);

            var monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var yearStart = new DateTime(DateTime.Now.Year, 1, 1);

            DeviceList = DeviceCache;

            PredefinedDateFilter = new Dictionary<string, string>
            {
                { "Today", DateTime.Today.Subtract(unixEpoch).TotalMilliseconds.ToString() },
                { "Yesterday", DateTime.Today.AddDays(-1).Subtract(unixEpoch).TotalMilliseconds.ToString() },
                { "This Week", $"{weekStart.Subtract(unixEpoch).TotalMilliseconds}/{weekStart.AddDays(6).Subtract(unixEpoch).TotalMilliseconds}" },
                { "Last Week", $"{weekStart.AddDays(-7).Subtract(unixEpoch).TotalMilliseconds }/{weekStart.AddDays(-1).Subtract(unixEpoch).TotalMilliseconds}" },
                { "This Month", $"{monthStart.Subtract(unixEpoch).TotalMilliseconds}/{monthStart.AddDays(DateTime.DaysInMonth(monthStart.Year, monthStart.Month)).AddDays(-1).Subtract(unixEpoch).TotalMilliseconds}" },
                { "Last Month", $"{monthStart.AddMonths(-1).Subtract(unixEpoch).TotalMilliseconds}/{monthStart.AddMonths(-1).AddDays(DateTime.DaysInMonth(monthStart.AddMonths(-1).Year, monthStart.AddMonths(-1).Month)).AddDays(-1).Subtract(unixEpoch).TotalMilliseconds}" },
                { "This Year" , $"{yearStart.Subtract(unixEpoch).TotalMilliseconds}/{yearStart.AddMonths(12).AddDays(-1).Subtract(unixEpoch).TotalMilliseconds}" },
                { "Last Year", $"{yearStart.AddYears(-1).Subtract(unixEpoch).TotalMilliseconds}/{yearStart.AddYears(-1).AddMonths(12).AddDays(-1).Subtract(unixEpoch).TotalMilliseconds}" }
            };

            WebsiteList = WebsiteCache;
        }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Device")]
        public IEnumerable<string> DeviceList { get; set; }

        [Display(Name = "Period Mask")]
        public PeriodFilterForm PeriodForm { get; set; }

        [Display(Name = "Quick period filter")]
        public Dictionary<string, string> PredefinedDateFilter { get; set; }

        public IEnumerable<OperatorReportItem> Rows { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Website")]
        public IEnumerable<string> WebsiteList { get; set; }
    }
}