using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }

        [StringLength(100)]
        public string Tags { get; set; }



        [NotMapped]
        public ICollection<BlogComment>? Comments { get; set; }
        [NotMapped]
        public ICollection<Interaction>? Interactions { get; set; }


    }
}
