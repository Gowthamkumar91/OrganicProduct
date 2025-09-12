using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace OrganicProduct.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Name can't exceed 100 characters")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description can't exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 100000, ErrorMessage = "Enter a valid price between ₹0.01 and ₹1,00,000")]
        [Display(Name = "Price (₹)")]
        public decimal Price { get; set; }

        [Range(0, 10000, ErrorMessage = "Stock must be between 0 and 10,000")]
        [Display(Name = "Available Stock")]
        public int Stock { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage ="Category is Required")]
        [Display(Name = "Product Category")]
        public string Category { get; set; }

       
    }
}
