using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Powerfront.BackendTest.Controllers
{
    public class DefaultApiController : ApiController
    {
        // GET: api/DefaultApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/DefaultApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/DefaultApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/DefaultApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/DefaultApi/5
        public void Delete(int id)
        {
        }
    }
}
