using System.Linq;
using Microsoft.AspNet.Mvc;

namespace POC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }                
    }
}