using System.ComponentModel.DataAnnotations;

namespace RF_Technologies.Model.VM
{
    public class LoginVM
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RemeberMe { get; set; }

        public string? RedirectUrl { get; set; }

        public string? LockoutMessage { get; set; } // Add this property
    }
}
