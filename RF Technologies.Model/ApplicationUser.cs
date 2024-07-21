using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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

        public string FacebookLink { get; set; }
        public string LinkedinLink { get; set; }
        public string XLink { get; set; }
        public string YouTubeLink { get; set; }
        public string InstagramLink { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }
        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }

        [NotMapped]
        public ICollection<BlogComment> Comments { get; set; }
        [NotMapped]
        public ICollection<Interaction> Interactions { get; set; }
        [NotMapped]
        public ICollection<BlogPost> Posts { get; set; }
    }
}
