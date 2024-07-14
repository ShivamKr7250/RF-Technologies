using Google.Apis.YouTube.v3.Data;
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
        public DbSet<BlogPost> Blogs { get; set; }
        public DbSet<BlogComment> BlogsComment { get; set; }
        public DbSet<Interaction> Interaction { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<BlogPost>()
                .HasKey(p => p.PostId);

            modelBuilder.Entity<BlogComment>()
                .HasKey(c => c.CommentId);

            modelBuilder.Entity<BlogComment>()
                .HasOne(c => c.BlogPost)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);

            modelBuilder.Entity<BlogComment>()
                .HasOne(c => c.ApplicationUser)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Interaction>()
                .HasKey(i => i.InteractionId);

            modelBuilder.Entity<Interaction>()
                .HasOne(i => i.BlogPost)
                .WithMany(p => p.Interactions)
                .HasForeignKey(i => i.PostId);

            modelBuilder.Entity<Interaction>()
                .HasOne(i => i.ApplicationUser)
                .WithMany(u => u.Interactions)
                .HasForeignKey(i => i.UserId);
        }


    }
}
