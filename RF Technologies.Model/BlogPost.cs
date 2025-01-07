using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RF_Technologies.Model
{
    public class BlogPost
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public BlogCategory? BlogCategory { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(400)]
        public string ShortDescription { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }
        [Display(Name = "Blog Thubnail")]
        public string BlogThumnail { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }

        [StringLength(100)]
        public string Tags { get; set; }

        [NotMapped]
        public string AuthorName { get; set; }

        [NotMapped]
        public ICollection<BlogComment> Comments { get; set; }
        [NotMapped]
        public ICollection<Interaction> Interactions { get; set; }

    }
}
