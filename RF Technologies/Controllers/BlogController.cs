using Microsoft.AspNetCore.Mvc;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;
using System.Security.Claims;

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
            var postFromDb = _unitOfWork.BlogPost.GetAll(includeProperties: "ApplicationUser,Comments");
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID
            var model = new BlogPost
            {
                UserId = userId
            };

            return View(model);
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

        [HttpGet]
        public IActionResult Details(int blogId)
        {
            var blogDetails = _unitOfWork.BlogPost.Get(u => u.PostId == blogId);
            if(blogDetails == null)
            {
                return BadRequest();
            }
            else
            {
                return View(blogDetails);
            }
            
        }
    }
}
