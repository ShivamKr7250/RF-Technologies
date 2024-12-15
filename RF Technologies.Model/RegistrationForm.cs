using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RF_Technologies.Model
{
    public class RegistrationForm
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string? Resume { get; set; }

        [Required]
        public string Domain { get; set; }

        [Required]
        public string ContactNo { get; set; }

        [Required]
        public string Linkedin { get; set; }

        [Required]
        public string GitHub { get; set; }

        [Required]
        public string DateOfBirth { get; set; }

        [Required]
        public string Education { get; set; }

        [Required]
        public string CollegeName { get; set; }

        [Required]
        public string Skill { get; set; }

        public string? Status { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
