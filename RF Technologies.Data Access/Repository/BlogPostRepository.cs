using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RF_Technologies.Data_Access.Data;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;

namespace RF_Technologies.Data_Access.Repository
{
    public class BlogPostRepository : Repository<BlogPost> , IBlogPostRepository
    {
        private readonly ApplicationDbContext _db;
        public BlogPostRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(BlogPost entity)
        {
            _db.Update(entity);
        }

        public IEnumerable<BlogPost> GetPostsByDescendingPublicationDate()
        {
            return _db.Blogs
                           .Include(bp => bp.ApplicationUser) // Ensure that ApplicationUser is included
                           .OrderByDescending(bp => bp.PublicationDate)
                           .ToList();
        }


    }
}
