using RF_Technologies.Model;

namespace RF_Technologies.Data_Access.Repository.IRepository
{
    public interface IBlogCategoryRepository : IRepository<BlogCategory>
    {
        void Update(BlogCategory entity);
    }
}
