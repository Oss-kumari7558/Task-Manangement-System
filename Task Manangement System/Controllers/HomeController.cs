using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Task_Manangement_System.Models;

namespace Task_Manangement_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
          return RedirectToAction("Index", "TaskManagementController1");

        }

       
    }
}
