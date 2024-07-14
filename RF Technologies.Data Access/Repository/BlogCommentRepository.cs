using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RF_Technologies.Data_Access.Data;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;

namespace RF_Technologies.Data_Access.Repository
{
    public class BlogCommentRepository : Repository<BlogComment> , IBlogCommentRepository
    {
        private readonly ApplicationDbContext _db;
        public BlogCommentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(BlogComment entity)
        {
            _db.Update(entity);
        }
    }
}
