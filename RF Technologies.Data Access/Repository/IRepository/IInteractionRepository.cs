using RF_Technologies.Model;

namespace RF_Technologies.Data_Access.Repository.IRepository
{
    public interface IInteractionRepository : IRepository<Interaction>
    {
        void Update(Interaction entity);
    }
}
