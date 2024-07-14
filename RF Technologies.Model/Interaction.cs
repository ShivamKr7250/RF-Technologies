using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RF_Technologies.Model
{
    public class Interaction
    {
        [Key]
        public int InteractionId { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; }

        [Required]
        public int? PostId { get; set; }
        [ForeignKey("PostId")]
        public BlogPost? BlogPost { get; set; }

        [Required]
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
