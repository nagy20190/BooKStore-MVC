using BKStore_MVC.Models;
using BKStore_MVC.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BKStore_MVC.Controllers
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
            return View("Index");
        }

        public IActionResult AboutUs()
        {
            return View("AboutUs");
        }

        public IActionResult ContactUs()
        {
            return View("ContactUs");
        }

        [HttpPost]
        public IActionResult SubmitContact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Handle form submission, e.g., send an email or save to the database
                // Redirect to a confirmation page or back to the contact page
                return RedirectToAction("ContactConfirmation");
            }

            return View("Contact", model);
        }

        public IActionResult ContactConfirmation()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View("Privacy");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
