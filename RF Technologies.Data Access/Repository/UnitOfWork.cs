using RF_Technologies.Data_Access.Data;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;

namespace RF_Technologies.Data_Access.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IRegistrationFormRepository RegistrationForm {  get; private set; }
        public IApplicationUserRepository User {  get; private set; }
        public IContactRepository Contact { get; private set; }
        public IInternshipSubmitRepository InternshipSubmit { get; private set; }
        public IBlogPostRepository BlogPost { get; private set; }
        public IBlogCommentRepository BlogComment { get; private set; }
        public IInteractionRepository Interaction { get; private set; }
        public IBlogCategoryRepository BlogCategory { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            RegistrationForm = new RegistrationFormRepository(_db);
            User = new ApplicationUserRepository(_db);
            Contact = new ContactRepository(_db);
            InternshipSubmit = new InternshipSubmitRepository(_db);
            BlogPost = new BlogPostRepository(_db);
            BlogComment = new BlogCommentRepository(_db);
            Interaction = new InteractionRepository(_db);
            BlogCategory = new BlogCategoryRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
