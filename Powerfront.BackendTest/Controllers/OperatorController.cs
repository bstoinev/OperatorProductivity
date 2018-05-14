using Powerfront.BackendTest;
using Powerfront.BackendTest.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Powerfront.BackendTest.Controllers
{
    public class OperatorController : Controller
    {
        private IEnumerable<OperatorReportItem> LoadReport(ProductivityReportCriteria filter)
        {
            var result = new List<OperatorReportItem>();

            var cs = ConfigurationManager.ConnectionStrings["chat"].ConnectionString;

            using (var conn = new SqlConnection(cs))
            using (var sqlcomm = conn.CreateCommand())
            {
                sqlcomm.CommandText = "dbo.OperatorProductivityRef";
                sqlcomm.CommandType = CommandType.StoredProcedure;

                if (filter != null)
                {
                    if (!string.IsNullOrWhiteSpace(filter.Device))
                    {
                        sqlcomm.Parameters
                            .AddWithValue("@Device", filter.Device)
                            .SqlDbType = SqlDbType.VarChar;
                    }

                    if (filter.EndDate.HasValue)
                    {
                        sqlcomm.Parameters
                            .AddWithValue("@EndDate", filter.EndDate.Value)
                            .SqlDbType = SqlDbType.DateTime2;
                    }

                    if (filter.StartDate.HasValue)
                    {
                        sqlcomm.Parameters
                            .AddWithValue("@StartDate", filter.StartDate)
                            .SqlDbType = SqlDbType.DateTime2;
                    }

                    if (!string.IsNullOrWhiteSpace(filter.Website))
                    {
                        sqlcomm.Parameters
                            .AddWithValue("@Website", filter.Website)
                            .SqlDbType = SqlDbType.VarChar;
                    }
                }

                conn.Open();

                var dr = sqlcomm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var reportItem = new OperatorReportItem();
                        reportItem.ID = dr.GetInt32(0);
                        reportItem.Name = dr.GetString(1);
                        reportItem.ProactiveSent = dr.IsDBNull(2) ? 0 : (int)dr[2];
                        reportItem.ProactiveAnswered = dr.IsDBNull(3) ? 0 : dr.GetInt32(3);
                        reportItem.ReactiveReceived = dr.IsDBNull(4) ? 0 : dr.GetInt32(4);
                        reportItem.ReactiveAnswered = dr.IsDBNull(5) ? 0 : dr.GetInt32(5);
                        reportItem.TotalChatLengthSeconds = dr.IsDBNull(6) ? 0 : dr.GetInt32(6);
                        reportItem.AverageChatLengthSeconds = dr.IsDBNull(7) ? 0 : dr.GetInt32(7);

                        if (reportItem.ProactiveSent != 0)
                        {
                            reportItem.ProactiveResponseRate = Convert
                                .ToInt32((double)reportItem.ProactiveAnswered / reportItem.ProactiveSent * 100);
                        }

                        if (reportItem.ReactiveReceived != 0)
                        {
                            reportItem.ReactiveResponseRate = Convert
                                .ToInt32((double)reportItem.ReactiveAnswered / reportItem.ReactiveReceived * 100);
                        }

                        result.Add(reportItem);
                    }
                }
            }

            return result;
        }

        [HttpGet]
        public ActionResult ProductivityReport()
        {
            var model = new OperatorReportViewModelRef();

            ViewBag.Message = "Operator Productivity Report";

            return View(model);
        }

        public ActionResult ProductivityReportTable(ProductivityReportCriteria filter)
        {
            ActionResult result = null;

            try
            {
                var model = LoadReport(filter);
                result = PartialView(model);
            }
            catch (SqlException se)
            {
                Trace.WriteLine($"Failed to open the data store, due to exception:{Environment.NewLine}{se}");
                result = new HttpStatusCodeResult(400, se.Message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Failed to load the report, due to exception:{Environment.NewLine}{ex}");
                result = new HttpStatusCodeResult(500, "The report cannot be generated. Try again later.");
            }

            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult ProductivityReportCsv(string filterDump)
        {
            var filter = JsonConvert.DeserializeObject<ProductivityReportCriteria>(filterDump);

            ActionResult result = null;
            IEnumerable<OperatorReportItem> data = null;

            try
            {
                data = LoadReport(filter);
            }
            catch (SqlException se)
            {
                Trace.WriteLine($"Failed to open the data store, due to exception:{Environment.NewLine}{se}");
                result = new HttpStatusCodeResult(400, se.Message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Failed to load the report, due to exception:{Environment.NewLine}{ex}");
                result = new HttpStatusCodeResult(500, "The report cannot be generated. Try again later.");
            }

            if (data != null)
            {
                var sb = new StringBuilder();


                var lines = $"Operator ID,Name,Proactive Sent,Proactive Answered,Proactive Response Rate [%],Reactive Received,Reactive Answered,Reactive Response Rate [%],Total Chat Length [sec],Average Chat Length[sec]{Environment.NewLine}";

                lines += data.Aggregate(string.Empty, 
                    (c, n) => c += $"{n.ID},{n.Name},{n.ProactiveSent},{n.ProactiveAnswered},{n.ProactiveResponseRate},{n.ReactiveReceived},{n.ReactiveAnswered},{n.ReactiveResponseRate},{n.TotalChatLengthSeconds},{n.AverageChatLengthSeconds}{Environment.NewLine}");

                result = File(Encoding.UTF8.GetBytes(lines), "text/csv", "ProductivityReport.csv");
            }

            return result;
        }
    }
}