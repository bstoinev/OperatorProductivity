using System.Web.Mvc;

namespace Powerfront.BackendTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var controllerName = nameof(OperatorController);

            controllerName = controllerName.Substring(0, controllerName.IndexOf("Controller"));

            return RedirectToAction(nameof(OperatorController.ProductivityReport), controllerName);
        }
    }
}