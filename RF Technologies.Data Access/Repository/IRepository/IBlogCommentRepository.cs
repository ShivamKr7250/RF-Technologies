using RF_Technologies.Model;

namespace RF_Technologies.Data_Access.Repository.IRepository
{
    public interface IBlogCommentRepository : IRepository<BlogComment>
    {
        void Update(BlogComment entity);
    }
}
