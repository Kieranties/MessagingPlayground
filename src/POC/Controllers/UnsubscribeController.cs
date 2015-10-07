using System.Linq;
using Microsoft.AspNet.Mvc;
using POC.Integration.Workflows;
using POC.Messages.Commands;
using System.Messaging;
using Newtonsoft.Json;
using System.Text;
using System.IO;

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
            //var workflow = new UnsubscribeWorkflow(emailAddress);
            //workflow.Run();

            var unsubscribeCommand = new UnsubscribeCommand
            {
                EmailAddress = emailAddress
            };

            using(var queue = new MessageQueue(".\\private$\\poc.messagequeue.unsubscribe"))
            {
                // serialize using default xml serialization
                // var message = new Message(unsubscribeCommand); //

                // serailize to json and write raw bytes
                var message = new Message();
                var json = JsonConvert.SerializeObject(unsubscribeCommand);
                message.BodyStream = new MemoryStream(Encoding.Default.GetBytes(json));
                queue.Send(message);

                // json raw bytes cleraly reduces size of message
            }

            return View("Confirmation");
        }
    }
}