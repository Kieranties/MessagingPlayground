using System.Linq;
using Microsoft.AspNet.Mvc;
using POC.Integration.Workflows;
using POC.Messages.Commands;
using System.Messaging;

namespace POC.Controllers
{
    [Route("/[controller]")]
    public class UnsubscribeController : Controller
    {
        [HttpGet]        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(string emailAddress)
        {
            var workflow = new UnsubscribeWorkflow(emailAddress);
            workflow.Run();

            return View("Confirmation");
        }
    }
}