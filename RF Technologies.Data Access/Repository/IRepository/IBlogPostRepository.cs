using RF_Technologies.Model;

namespace RF_Technologies.Data_Access.Repository.IRepository
{
    public interface IBlogPostRepository : IRepository<BlogPost>
    {
        void Update(BlogPost entity);

        IEnumerable<BlogPost> GetPostsByDescendingPublicationDate();
    }
}
