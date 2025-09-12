using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OrganicProduct.Models;
using System.Data;

namespace OrganicProduct.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
        public IActionResult Shop(string category)
        {
            var products = new List<Product>();
            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("GetAllProducts", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var p = new Product
                    {
                        ProductId = (int)reader["ProductId"],
                        Name = reader["Name"].ToString(),
                        Price = (decimal)reader["Price"],
                        ImageUrl = reader["ImageUrl"].ToString(),
                        Category = reader["Category"].ToString()
                    };

                    if (string.IsNullOrEmpty(category) || p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                        products.Add(p);
                }
            }

            ViewBag.SelectedCategory = category;
            return View(products);
        }

    }
}
