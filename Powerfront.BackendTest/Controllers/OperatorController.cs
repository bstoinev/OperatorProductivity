using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Powerfront.BackendTest.Controllers
{
    public class OperatorController : ApiController
    {
        public IEnumerable<OperatorReportItem> ProductivityReport(ProductivityReportCriteria filter)
        {
            throw new NotImplementedException();
        }
    }
}
