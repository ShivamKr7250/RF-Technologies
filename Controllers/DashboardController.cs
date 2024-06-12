using Microsoft.AspNetCore.Mvc;

namespace RF_Technologies.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
