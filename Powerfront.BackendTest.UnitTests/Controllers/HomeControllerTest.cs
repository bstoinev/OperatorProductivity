using Microsoft.VisualStudio.TestTools.UnitTesting;
using Powerfront.BackendTest.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Powerfront.BackendTest.UnitTests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        /// <summary>
        /// Should return <see cref="RedirectToRouteResult"/>.
        /// Should redirect to <see cref="BackendTest.Controllers.OperatorController.ProductivityReport"/>
        /// </summary>
        [TestMethod]
        public void Index()
        {
            var cntrlr = new HomeController();
            var result = cntrlr.Index();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            var redir = (RedirectToRouteResult)result;

            var expectedCntrlrName = nameof(BackendTest.Controllers.OperatorController);
            var expectedActionName = nameof(BackendTest.Controllers.OperatorController.ProductivityReport);

            var actualCntrlr = redir.RouteValues.FirstOrDefault(rv => rv.Key.Equals("controller", StringComparison.OrdinalIgnoreCase));
            var actualAction = redir.RouteValues.FirstOrDefault(rv => rv.Key.Equals("action", StringComparison.OrdinalIgnoreCase));

            expectedCntrlrName = expectedCntrlrName.Substring(0, expectedCntrlrName.IndexOf("Controller"));

            Assert.AreEqual(expectedCntrlrName, actualCntrlr.Value);
            Assert.AreEqual(expectedActionName, actualAction.Value);
        }

    }
}
