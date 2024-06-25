using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RF_Technologies.Model
{
    public class InternshipSubmit
    {
        [Key]
        public int ID { get; set; }

        public string GitHubLink1 { get; set; }
        public string LinkedinLink1 { get; set; }

        public string GitHubLink2 { get; set; }
        public string LinkedinLink2 { get; set; }

        public string GitHubLink3 { get; set; }
        public string LinkedinLink3 { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }
        [Required]
        public string PaymentScreenShot {  get; set; }

        [Required]
        public int? IntenshipId { get; set; }
        [ForeignKey("IntenshipId")]
        public RegistrationForm? RegistrationForm { get; set; }
    }
}
