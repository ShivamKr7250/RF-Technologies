using RF_Technologies.Data_Access.Data;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;
using RF_Technologies.Utility;

namespace RF_Technologies.Data_Access.Repository
{
    public class RegistrationFormRepository : Repository<RegistrationForm>, IRegistrationFormRepository
    {
        private readonly ApplicationDbContext _db;

        public RegistrationFormRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public void Update(RegistrationForm entity)
        {
            _db.Update(entity);
        }

        public void UpdateStatus(int registrationId, string registrationStatus)
        {
            var registrationFromDb = _db.RegistrationForms.FirstOrDefault(m => m.ID == registrationId);
            if (registrationFromDb != null)
            {
                registrationFromDb.Status = registrationStatus;
                if (registrationStatus == SD.StatusApproved)
                {
                    DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
                    registrationFromDb.StartDate = currentDate.AddDays(2);

                    DateOnly starDate = DateOnly.FromDateTime(DateTime.Now);
                    registrationFromDb.EndDate = starDate.AddMonths(1);
                }

                if (registrationStatus == SD.StatusInternshipSubmited)
                {
                   registrationFromDb.Status = SD.StatusCompleted;
                }
            }
        }
    }
}
