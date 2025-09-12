using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace OrganicProduct.Models.Controllers
{
    public class ProductAdminController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProductAdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public IActionResult Index()
        {
            List<Product> products = new List<Product>();
            using(var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("GetAllProducts", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = (int)reader["productid"],
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Price = (decimal)reader["Price"],
                        Stock = (int)reader["Stock"],
                        ImageUrl = reader["ImageUrl"].ToString(),
                        Category = reader["Category"].ToString()
                    });
                }
            }

            return View(products);
        }

        // GET: /ProductAdmin/Details/5
        public IActionResult Details(int id)
        {
            Product product = new Product();
            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("GetProductById", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ProductId", id);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    product.ProductId = (int)reader["ProductId"];
                    product.Name = reader["Name"].ToString();
                    product.Description = reader["Description"].ToString();
                    product.Price = (decimal)reader["Price"];
                    product.Stock = (int)reader["Stock"];
                    product.ImageUrl = reader["ImageUrl"].ToString();
                    product.Category = reader["Category"].ToString();
                }
                else
                {
                    return NotFound(); 
                }
            }

            return View(product); 
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Product product, IFormFile ImageFile)
        {
            string imagePath = product.ImageUrl;
          

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                imagePath = "/uploads/" + fileName; // Save relative path
               
            }
            Debug.WriteLine("Name: " + product.Name);
            Debug.WriteLine("Price: " + product.Price);
            Debug.WriteLine("Description: " + product.Description);
            Debug.WriteLine("Stock: " + product.Stock);
            Debug.WriteLine("Image Path: " + imagePath);
            Debug.WriteLine("Category: " + product.Category);

            product.ImageUrl = imagePath ?? "";

            ModelState.Remove("ImageUrl");

            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Debug.WriteLine($"Model error on {entry.Key}: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                using (var con = GetConnection())
                {
                    
                    con.Open();

                    SqlCommand cmd = new SqlCommand("AddProduct", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue ("@Description", product.Description ?? "");
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Stock",product.Stock);
                    cmd.Parameters.AddWithValue("@ImageUrl",product.ImageUrl ??"");
                    cmd.Parameters.AddWithValue("@Category",product.Category);
               

                    cmd.ExecuteNonQuery();
                };
                return RedirectToAction("Index");
                
            }
            return View(product);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Product product = new Product();
            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("GetProductById", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ProductId", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    product.ProductId = (int)reader["ProductId"];
                    product.Name = reader["Name"].ToString();
                    product.Description = reader["Description"].ToString();
                    product.Price = (decimal)reader["Price"];
                    product.Stock = (int)reader["Stock"];
                    product.Category = reader["Category"].ToString();
                }
            }
            return View(product); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Product product) 
        {
            using (var con = GetConnection())
            { 
                con.Open();
                SqlCommand cmd = new SqlCommand("DeleteProduct", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@productId",product.ProductId);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");

           
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Product product = new Product();
            using (var con = GetConnection()) {
                con.Open();
                SqlCommand cmd = new SqlCommand("GetProductById", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@productId", id);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) 
                {
                    product.ProductId = (int)reader["productid"];
                    product.Name = reader["Name"].ToString();
                    product.Description = reader["Description"].ToString();
                    product.Price = (decimal)reader["Price"];
                    product.Stock = (int)reader["Stock"];
                    product.ImageUrl = reader["ImageUrl"].ToString();
                    product.Category = reader["Category"].ToString();
                }
            }

            return View(product);
        
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product, IFormFile ImageFile) 
        {
            string imagePath = product.ImageUrl;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                var extension = Path.GetExtension(ImageFile.FileName);
                var newFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
              

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                product.ImageUrl = "/uploads/" + newFileName;
            }
            else
            {
                // Important: retain old image if no new one uploaded
                product.ImageUrl = imagePath;
            }
            Debug.WriteLine("----- EDIT DEBUG -----");
            Debug.WriteLine("ModelState valid: " + ModelState.IsValid);
            foreach (var modelError in ModelState)
            {
                foreach (var error in modelError.Value.Errors)
                {
                    Debug.WriteLine($"Error in {modelError.Key}: {error.ErrorMessage}");
                }
            }
            Debug.WriteLine("ProductId: " + product.ProductId);
            Debug.WriteLine("Name: " + product.Name);
            Debug.WriteLine("Price: " + product.Price);
            Debug.WriteLine("Stock: " + product.Stock);
            Debug.WriteLine("Description: " + product.Description);
            Debug.WriteLine("ImageUrl: " + product.ImageUrl);
            Debug.WriteLine("ImageFile: " + (ImageFile != null ? ImageFile.FileName : "NULL"));
            Debug.WriteLine("Category: " + product.Category);

            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                
                using (var con = GetConnection())
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("UpdateProduct", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description ?? "");
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Stock", product.Stock);
                    cmd.Parameters.AddWithValue("@ImageUrl", product.ImageUrl ?? "");
                    cmd.Parameters.AddWithValue("@Category",product.Category ?? "");

                    cmd.ExecuteNonQuery();
                };
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("Index");

            }
            return View(product);

        }


        public IActionResult ManageStock()
        {
            List<Product> lowStockProducts = new List<Product>();
            using (var con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE Stock < 10", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lowStockProducts.Add(new Product
                    {
                        ProductId = (int)reader["ProductId"],
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Price = (decimal)reader["Price"],
                        Stock = (int)reader["Stock"],
                        ImageUrl = reader["ImageUrl"].ToString(),
                        Category = reader["Category"].ToString()
                    });
                }
            }

            return View(lowStockProducts);
        }


        public IActionResult OrderManagement()
        {
            return View();
        }
    }
}
