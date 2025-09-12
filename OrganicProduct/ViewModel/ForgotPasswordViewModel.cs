using System.ComponentModel.DataAnnotations;

namespace OrganicProduct.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
