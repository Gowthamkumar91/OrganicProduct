using Microsoft.AspNetCore.Identity;

namespace OrganicProduct.Data
{
    public class ApplicationUser :IdentityUser
    {
        public string? FullName {  get; set; }
    }
}
