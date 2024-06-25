using RF_Technologies.Model;

namespace RF_Technologies.Data_Access.Repository.IRepository
{
    public interface IInternshipSubmitRepository : IRepository<InternshipSubmit>
    {
        void Update(InternshipSubmit entity);

        void UpdateStatus(int internshipId, string internshipStatus);
    }
}
