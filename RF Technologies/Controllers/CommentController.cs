using Microsoft.AspNetCore.Mvc;

namespace RF_Technologies.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
