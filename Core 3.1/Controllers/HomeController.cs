using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Core_3._1.Models;
using Microsoft.AspNetCore.Authorization;

namespace Core_3._1.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
