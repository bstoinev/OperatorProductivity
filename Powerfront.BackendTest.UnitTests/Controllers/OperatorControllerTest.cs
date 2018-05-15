using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Powerfront.BackendTest.Controllers;
using Powerfront.BackendTest.Models;

namespace Powerfront.BackendTest.UnitTests.Controllers
{
    [TestClass]
    public class OperatorControllerTest
    {

        /// <summary>
        /// Should return <see cref="ViewResult"/>
        /// View name should default to the action name - i.e. <see cref="string.Empty"/>
        /// View model should be of type <see cref="OperatorReportViewModelRef"/>
        /// </summary>
        [TestMethod]
        public void ProductivityReport()
        {
            var cntrlr = new OperatorController();
            var result = cntrlr.ProductivityReport();

            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var view = (ViewResult)result;

            Assert.AreEqual(string.Empty, view.ViewName);

            Assert.IsInstanceOfType(view.Model, typeof(OperatorReportViewModelRef));
        }
    }
}
