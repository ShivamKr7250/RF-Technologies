﻿using RF_Technologies.Model;

namespace RF_Technologies.Data_Access.Repository.IRepository
{
    public interface IContactRepository : IRepository<Contact>
    {
        void Update(Contact entity);
    }
}
