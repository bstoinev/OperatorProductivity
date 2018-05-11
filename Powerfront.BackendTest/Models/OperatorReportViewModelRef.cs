using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Powerfront.BackendTest.Models
{
    public class OperatorReportViewModelRef
    {
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
            DeviceList = DeviceCache;

            PredefinedDateFilter = new string[]
            {
                "Today",
                "Yesterday",
                "This Week",
                "Last Week",
                "This Month",
                "Last Month",
                "This Year",
                "Last Year",
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
        public IEnumerable<string> PredefinedDateFilter { get; set; }

        public IEnumerable<OperatorReportItem> Rows { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Website")]
        public IEnumerable<string> WebsiteList { get; set; }
    }
}