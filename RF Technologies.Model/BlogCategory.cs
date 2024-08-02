using System.ComponentModel.DataAnnotations;

namespace RF_Technologies.Model
{
    public class BlogCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

    }
}
