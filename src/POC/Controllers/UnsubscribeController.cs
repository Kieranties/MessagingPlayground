using System.Linq;
using Microsoft.AspNet.Mvc;
using POC.Messages;
using POC.Messages.Commands;
using System.Messaging;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System;
using POC.Messages.Queries;

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
            if (DoesUserExist(emailAddress))
            {
                StartUnsubscribe(emailAddress);
                return View("Confirmation");
            }

            return View("Unknown");

        }

        /// <summary>
        /// Sends a message to check for the existance of a user using Request-Response pattern
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        private bool DoesUserExist(string emailAddress)
        {
            // create a private response queue
            var responseAddress = Guid.NewGuid().ToString().Substring(0, 6);
            responseAddress = ".\\private$\\" + responseAddress;

            try
            {                
                using (var responseQueue = MessageQueue.Create(responseAddress))
                {
                    var doesUserExist = new DoesUserExistRequest
                    {
                        EmailAddress = emailAddress
                    };

                    using (var requestQueue = new MessageQueue(".\\private$\\poc.messagequeue.doesuserexist"))
                    {
                        var message = new Message
                        {
                            BodyStream = doesUserExist.ToJsonStream(),
                            Label = doesUserExist.GetMessageType(),
                            ResponseQueue = responseQueue // set the response queue to expect a response on
                        };
                        requestQueue.Send(message);
                    }

                    var response = responseQueue.Receive(); // handle the response (blocking call)
                    var responseBody = response.BodyStream.FromJson<DoesUserExistResponse>();
                    
                    return responseBody.Exists;
                }
            }
            finally // dispose of message queue if failure occurs
            {
                if (MessageQueue.Exists(responseAddress))
                {
                    MessageQueue.Delete(responseAddress);
                }
            }
        }

        private void StartUnsubscribe(string emailAddress)
        {
            var unsubscribeCommand = new UnsubscribeCommand
            {
                EmailAddress = emailAddress
            };

            using (var queue = new MessageQueue(".\\private$\\poc.messagequeue.unsubscribe"))
            {
                // serailize to json and write raw bytes
                var message = new Message
                {
                    BodyStream = unsubscribeCommand.ToJsonStream(),
                    Label = unsubscribeCommand.GetMessageType()
                };

                queue.Send(message);
            }
        }
    }
}