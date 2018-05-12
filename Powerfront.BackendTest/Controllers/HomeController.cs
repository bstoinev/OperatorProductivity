using Newtonsoft.Json;
using OperatorReport.Models;
using Powerfront.BackendTest;
using Powerfront.BackendTest.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Mvc;

namespace OperatorReport.Controllers
{
    public class HomeController : Controller
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

                conn.Open();

                var dr = sqlcomm.ExecuteReader();

                if (dr != null && dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var reportItem = new OperatorReportItem();
                        reportItem.ID = dr.GetInt32(0);
                        reportItem.Name = dr.GetString(1);
                        reportItem.ProactiveSent = Convert.IsDBNull(dr[2]) ? 0 : dr.GetInt32(2);
                        reportItem.ProactiveAnswered = Convert.IsDBNull(dr[3]) ? 0 : dr.GetInt32(3);

                        reportItem.ProactiveResponseRate = reportItem.ProactiveSent == 0
                            ? 0
                            : Convert.ToInt32((double)reportItem.ProactiveAnswered / reportItem.ProactiveSent * 100);

                        reportItem.ReactiveReceived = Convert.IsDBNull(dr[4]) ? 0 : dr.GetInt32(4);
                        reportItem.ReactiveAnswered = Convert.IsDBNull(dr[5]) ? 0 : dr.GetInt32(5);

                        reportItem.ReactiveResponseRate = reportItem.ReactiveReceived == 0
                            ? 0
                            : Convert.ToInt32((double)reportItem.ReactiveAnswered / reportItem.ReactiveReceived * 100);

                        reportItem.TotalChatLengthSeconds = Convert.IsDBNull(dr[6]) ? 0 : dr.GetInt32(6);
                        reportItem.AverageChatLengthSeconds = Convert.IsDBNull(dr[7]) ? 0 : dr.GetInt32(7);

                        result.Add(reportItem);
                    }
                }
            }

            return result;
        }

        public ActionResult Index()
        {
            return RedirectToAction(nameof(OperatorReport));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult OperatorReport()
        {
            var model = new OperatorReportViewModelRef();

            ViewBag.Message = "Operator Productivity Report";

            return View(model);
        }

        public ActionResult OperatorProductivityTable(ProductivityReportCriteria filter)
        {
            ActionResult result = null;

            try
            {
                var model = LoadReport(new ProductivityReportCriteria());
                result = PartialView("_ProductivityReportTable", model);
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

            return Json(result);
        }

        public ActionResult ProductivityReport()
        {
            return View();
        }
    }
}