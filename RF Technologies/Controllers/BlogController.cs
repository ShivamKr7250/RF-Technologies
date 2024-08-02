using Microsoft.AspNetCore.Mvc;
using RF_Technologies.Model;
using Microsoft.AspNetCore.Authorization;
using RF_Technologies.Model.VM;
using System.Security.Claims;
using System;
using RF_Technologies.Data_Access.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using static RF_Technologies.Controllers.UserController;
using RF_Technologies.Controllers.Service;
using Microsoft.AspNetCore.Mvc.Rendering;
using RF_Technologies.Data_Access.Repository;

namespace RF_Technologies.Controllers
{
    public class BlogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly BlogPostService _blogPostService;

        public BlogController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult BlogIndex()
        {
            return View();
        }

        public IActionResult Index()
        {
            var postFromDb = _unitOfWork.BlogPost.GetAll(includeProperties: "ApplicationUser,Comments");
            return View(postFromDb);
        }

        [Authorize]
        public IActionResult Create()
        {
            BlogVM blogVM = new BlogVM()
            {
                BlogPost = new BlogPost(),
                CategoryList = _unitOfWork.BlogCategory.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CategoryId.ToString()
                }),
                Tags = new List<string>()
            };
            return View(blogVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BlogVM model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userDetail = _unitOfWork.User.Get(u => u.Id == userId);
            model.BlogPost.UserId = userDetail.Id;
            model.BlogPost.AuthorName = userDetail.Name;
            model.BlogPost.PublicationDate = DateTime.Now;

            // Handle the blog thumbnail image
            if (model.BlogPost.Image != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.BlogPost.Image.FileName);
                string imagePath = Path.Combine(wwwRootPath, @"images\blogThumbnails");

                // Create the directory if it doesn't exist
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                string fullImagePath = Path.Combine(imagePath, fileName);
                using (var fileStream = new FileStream(fullImagePath, FileMode.Create))
                {
                    model.BlogPost.Image.CopyTo(fileStream);
                }

                model.BlogPost.BlogThumnail = @"\images\blogThumbnails\" + fileName;
            }

            // Handle tags functionality
            if (!string.IsNullOrEmpty(model.BlogPost.Tags))
            {
                model.BlogPost.Tags = string.Join(",", model.BlogPost.Tags);
            }

            _unitOfWork.BlogPost.Add(model.BlogPost);
            _unitOfWork.Save();
            TempData["success"] = "The Blog has been created successfully.";
            return RedirectToAction(nameof(Index));
        }

        

        [HttpGet]
        public IActionResult Details(int blogId)
        {
            // Include ApplicationUser for BlogPost and Comments
            var blogDetails = _unitOfWork.BlogPost.Get(u => u.PostId == blogId, includeProperties: "ApplicationUser,Comments.ApplicationUser");
            if (blogDetails == null)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userDetail = _unitOfWork.User.Get(u => u.Id == userId);

            var model = new BlogVM
            {
                BlogPost = blogDetails,
                BlogComment = new BlogComment
                {
                    UserId = userId,
                    ApplicationUser = userDetail,
                    PostId = blogId
                }
            };

            return View(model);
        }



        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(BlogVM model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.BlogComment.UserId = userId;
            model.BlogComment.Timestamp = DateTime.Now;
            _unitOfWork.BlogComment.Add(model.BlogComment);
            _unitOfWork.Save();
            TempData["success"] = "The Comment has been added successfully.";
            return RedirectToAction("Details", new { blogId = model.BlogComment.PostId });

            //var blogDetails = _unitOfWork.BlogPost.Get(u => u.PostId == model.BlogComment.PostId, includeProperties: "ApplicationUser,Comments");
            //model.BlogPost = blogDetails;
            //return View("Details", model);
        }

        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(new { success = false });
            }

            var results = _unitOfWork.BlogPost.SearchBlogPosts(searchTerm)
                .Select(b => new
                {
                    b.PostId,
                    b.Title,
                    b.ShortDescription,
                    b.PublicationDate
                })
                .ToList();

            return Json(new { success = true, data = results });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<BlogPost> objUserList = _unitOfWork.BlogPost.GetAll().ToList();
            return Json(new { data = objUserList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Invalid blog post ID" });
            }

            // Retrieve the blog post to be deleted
            var blogToBeDeleted = _unitOfWork.BlogPost.Get(u => u.PostId == id);
            if (blogToBeDeleted == null)
            {
                return Json(new { success = false, message = "Blog post not found" });
            }

            // Retrieve and delete all comments related to the blog post
            var commentsToBeDeleted = _unitOfWork.BlogComment.GetAll(c => c.PostId == id).ToList();
            foreach (var comment in commentsToBeDeleted)
            {
                _unitOfWork.BlogComment.Remove(comment);
            }

            // Delete the blog post
            _unitOfWork.BlogPost.Remove(blogToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted successfully" });
        }

        #endregion
    }
}
