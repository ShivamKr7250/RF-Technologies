using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_Technologies.Data_Access.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IRegistrationFormRepository RegistrationForm { get; }
        IApplicationUserRepository User { get; }
        IContactRepository Contact { get; }
        IInternshipSubmitRepository InternshipSubmit { get; }
        void Save();
    }
}
