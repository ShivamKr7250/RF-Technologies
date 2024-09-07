using Microsoft.AspNetCore.Mvc.Rendering;

namespace RF_Technologies.Model.VM
{
    public class BlogVM
    {
        public BlogPost BlogPost { get; set; }
        public IEnumerable<BlogPost> Post {  get; set; }
        public BlogComment BlogComment { get; set; }
        public ApplicationUser? User { get; set; }
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        public List<string>? Tags { get; set; }
        public IEnumerable<BlogComment>? Comments { get; set; }
    }
}
