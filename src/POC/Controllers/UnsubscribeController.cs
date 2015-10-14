using Microsoft.AspNet.Mvc;
using POC.Messages.Commands;
using POC.Messages.Queries;
using POC.Messaging;

namespace POC.Controllers
{
    [Route("/[controller]")]
    public class UnsubscribeController : Controller
    {
        private readonly IMessageQueue _unsubscribe;
        private readonly IMessageQueue _userExist;
        private readonly IMessageQueue _userExistResponse;

        public UnsubscribeController(IMessageQueueFactory queueFactory)
        {
            _userExist = queueFactory.Get("doesuserexist");
            _userExistResponse = _userExist.GetResponseQueue();
            _unsubscribe = queueFactory.Get("unsubscribe");
        }

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
            var exists = false;

            var doesUserExist = new DoesUserExistRequest { EmailAddress = emailAddress };                        
            var message = new Message
            {
                Body = doesUserExist,
                ResponseConnection = _userExistResponse.Connection
            };
            _userExist.Send(message);

            _userExistResponse.Receive(msg => exists = msg.BodyAs<DoesUserExistResponse>().Exists);
            
            return exists;            
        }

        private void StartUnsubscribe(string emailAddress)
        {
            var unsubscribeCommand = new UnsubscribeCommand { EmailAddress = emailAddress };            
            var message = new Message { Body = unsubscribeCommand };
            _unsubscribe.Send(message);
        }
    }
}