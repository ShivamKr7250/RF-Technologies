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

        //public IActionResult Index()
        //{
        //    var blogPosts = _unitOfWork.BlogPost.GetPostsByDescendingPublicationDate()
        //                    .Select(bp => new BlogPost
        //                    {
        //                        PostId = bp.PostId,
        //                        Title = bp.Title,
        //                        Content = bp.Content,
        //                        PublicationDate = bp.PublicationDate,
        //                        AuthorName = bp.ApplicationUser?.Name ?? "Unknown",
        //                        //CommentCount = bp.Comments?.Count() ?? 0
        //                    })
        //                    .ToList();

        //    return View(blogPosts);
        //}


        //public IActionResult Details(int? id)
        //{
        //    if (id == null) { 
        //        return View();
        //    }
        //    var post = _unitOfWork.BlogPost.Get(u => u.PostId == id, includeProperties: "ApplicationUser");
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(post);
        //}

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
            var userdetail = _unitOfWork.User.Get(u => u.Id == obj.UserId);
            obj.AuthorName = userdetail.Name;
            obj.PublicationDate = DateTime.Now;
            if (obj != null)
            {
     
                _unitOfWork.BlogPost.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "The Blog has been Created successfully.";
                return View();
            }
            else
            {
                return View("Index" ,"Home");
            }
        }

        [HttpGet]
        public IActionResult Details(int blogId)
        {
            var blogDetails = _unitOfWork.BlogPost.Get(u => u.PostId == blogId, includeProperties: "ApplicationUser");
            if (blogDetails == null)
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
