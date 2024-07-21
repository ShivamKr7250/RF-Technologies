using Microsoft.AspNetCore.Mvc;
using RF_Technologies.Data_Access.Repository.IRepository;

namespace RF_Technologies.Controllers
{
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(int id)
        {
            return RedirectToAction("Index" );
        }
    }
}
