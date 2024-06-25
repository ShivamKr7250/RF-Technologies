using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RF_Technologies.Model;

namespace RF_Technologies.Data_Access.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<RegistrationForm> RegistrationForms { get; set; }
        public DbSet<InternshipSubmit> SubmitedInternship { get; set; }
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<Contact> Contact { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


    }
}
