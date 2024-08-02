using Microsoft.AspNetCore.Mvc.Rendering;

namespace RF_Technologies.Model.VM
{
    public class BlogVM
    {
        public BlogPost BlogPost { get; set; }
        public BlogComment? BlogComment { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public List<string> Tags { get; set; }
    }
}
