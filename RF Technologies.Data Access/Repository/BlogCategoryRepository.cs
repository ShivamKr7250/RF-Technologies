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
    public class BlogCategoryRepository : Repository<BlogCategory> , IBlogCategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public BlogCategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(BlogCategory entity)
        {
            _db.Update(entity);
        }
    }
}
