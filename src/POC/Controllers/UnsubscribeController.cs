using System.Linq;
using Microsoft.AspNet.Mvc;
using POC.Integration.Workflows;

namespace POC.Controllers
{
    public class UnsubscribeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Submit(string emailAddress)
        {
            var workflow = new UnsubscribeWorkflow(emailAddress);
            workflow.Run();
            return View("Confirmation");
        }
    }
}