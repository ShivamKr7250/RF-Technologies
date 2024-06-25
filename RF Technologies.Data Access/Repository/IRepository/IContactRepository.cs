using RF_Technologies.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_Technologies.Data_Access.Repository.IRepository
{
    public interface IContactRepository : IRepository<Contact>
    {
        void Update(Contact entity);
    }
}
