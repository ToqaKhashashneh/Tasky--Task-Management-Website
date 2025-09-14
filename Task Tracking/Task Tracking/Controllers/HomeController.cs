using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Task_Tracking.Models;

namespace Task_Tracking.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDbContext _context;


       public HomeController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult About() {

            return View();
        }

        public IActionResult Contact ()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitContact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Contacts.Add(contact);
                _context.SaveChanges();
                return RedirectToAction("Contact");
            }
            return View("Contact", contact);

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
