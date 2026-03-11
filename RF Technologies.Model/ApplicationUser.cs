using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace RF_Technologies.Model
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public string Role { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string Bio { get; set; }

        // --- Academic & Professional Fields ---

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(150)]
        [Display(Name = "College/University Name")]
        public string CollegeName { get; set; }

        [StringLength(100)]
        [Display(Name = "Course/Degree")]
        public string Education { get; set; }

        [StringLength(50)]
        public string Domain { get; set; }

        [StringLength(300)]
        public string Skill { get; set; }

        // --- Social & Professional Links ---

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string GitHub { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string Linkedin { get; set; }

        [Url]
        public string FacebookLink { get; set; }

        [Url]
        public string XLink { get; set; }

        [Url]
        public string YouTubeLink { get; set; }

        [Url]
        public string InstagramLink { get; set; }

        // --- Image Handling ---

        [NotMapped]
        public IFormFile? Image { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }

        // --- Navigation Properties ---

        [NotMapped]
        public ICollection<BlogComment> Comments { get; set; }

        [NotMapped]
        public ICollection<Interaction> Interactions { get; set; }

        [NotMapped]
        public ICollection<BlogPost> Posts { get; set; }
    }
}