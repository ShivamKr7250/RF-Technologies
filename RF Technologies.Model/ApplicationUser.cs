using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF_Technologies.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
