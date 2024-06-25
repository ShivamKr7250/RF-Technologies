using RF_Technologies.Data_Access.Data;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;
using RF_Technologies.Utility;

namespace RF_Technologies.Data_Access.Repository
{
    public class InternshipSubmitRepository : Repository<InternshipSubmit>, IInternshipSubmitRepository
    {
        private readonly ApplicationDbContext _db;

        public InternshipSubmitRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public void Update(InternshipSubmit entity)
        {
            _db.Update(entity);
        }

        public void UpdateStatus(int internshipId, string internshipStatus)
        {
        }

    }
}
