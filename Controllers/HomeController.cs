using Microsoft.AspNetCore.Mvc;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;
using RF_Technologies.Models;
using System.Diagnostics;

namespace RF_Technologies.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Internship()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(Contact obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Contact.Add(obj);
                _unitOfWork.Save();
                return RedirectToAction("Index", "Home");
            }
            else { 
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
