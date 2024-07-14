using Microsoft.AspNetCore.Mvc;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;

namespace RF_Technologies.Controllers
{
    public class BlogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BlogController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var postFromDb = _unitOfWork.BlogPost.GetAll();
            return View(postFromDb);
        }

        public IActionResult Details(int? id)
        {
            if (id == null) { 
                return View();
            }
            var post = _unitOfWork.BlogPost.Get(u => u.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BlogPost obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.BlogPost.Add(obj);
                _unitOfWork.Save();
                TempData["MessageSent"] = true;
                return View();
            }
            else
            {
                return View();
            }
        }
    }
}
