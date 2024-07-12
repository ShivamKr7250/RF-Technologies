using System.ComponentModel.DataAnnotations;

namespace RF_Technologies.Model.VM
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
