using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RF_Technologies.Data_Access.Data;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;

namespace RF_Technologies.Data_Access.Repository
{
    public class ContactRepository : Repository<Contact> , IContactRepository
    {
        private readonly ApplicationDbContext _db;
        public ContactRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Contact entity)
        {
            _db.Update(entity);
        }
    }
}
