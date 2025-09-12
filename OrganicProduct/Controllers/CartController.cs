using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OrganicProduct.Extensions;
using OrganicProduct.Models;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace OrganicProduct.Controllers
{
    public class CartController : Controller
    {
        private readonly IConfiguration _configuration;

        public CartController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        // GET: /Cart
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<Cart>>("Cart") ?? new List<Cart>();
            return View(cart);
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        [Authorize]
       private void AddToCartByProductId(int productId)
        {
            var cart = HttpContext.Session.GetObject<List<Cart>>("Cart") ?? new List<Cart>();

            // Check if already in cart
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    var cmd = new SqlCommand("GetProductById", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@productId", productId);
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        cart.Add(new Cart
                        {
                            ProductId = (int)reader["ProductId"],
                            Name = reader["Name"].ToString(),
                            ImageUrl = reader["ImageUrl"].ToString(),
                            Price = (decimal)reader["Price"],
                            Quantity = 1
                        });
                    }
                }
            }

            HttpContext.Session.SetObject("Cart", cart);
           
       }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(int productId)
        {
            AddToCartByProductId(productId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddToCartRedirect(int productId)
        {
            Console.WriteLine($"AddToCartRedirect called with productId: {productId}");
            AddToCartByProductId(productId);
            return RedirectToAction("Index");
        }

        // Optional: Remove item
        public IActionResult Remove(int productId)
        {
            var cart = HttpContext.Session.GetObject<List<Cart>>("Cart") ?? new List<Cart>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
                cart.Remove(item);

            HttpContext.Session.SetObject("Cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult Increment(int productId)
        {
            var cart = HttpContext.Session.GetObject<List<Cart>>("Cart") ?? new List<Cart>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
                item.Quantity++;

            HttpContext.Session.SetObject("Cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult Decrement(int productId)
        {
            var cart = HttpContext.Session.GetObject<List<Cart>>("Cart") ?? new List<Cart>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                    cart.Remove(item);
            }

            HttpContext.Session.SetObject("Cart", cart);
            return RedirectToAction("Index");
        }

        


    }
}
