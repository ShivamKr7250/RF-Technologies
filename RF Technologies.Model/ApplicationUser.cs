using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RF_Technologies.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public string Role {  get; set; }

        [StringLength(200)]
        public string Bio { get; set; }

        public string ProfilePicture { get; set; }

        [NotMapped]
        public ICollection<BlogComment> Comments { get; set; }
        [NotMapped]
        public ICollection<Interaction> Interactions { get; set; }
    }
}
