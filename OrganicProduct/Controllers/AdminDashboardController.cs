using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace OrganicProduct.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IConfiguration _configuration;
        public AdminDashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            // Dummy values for portfolio(can replace with ADO.NET if needed)
            ViewBag.TotalProducts = 25;
            ViewBag.TotalOrders = 42;
            ViewBag.TotalUsers = 10;

            // Dummy low stock products (normally you get this from DB)
            var lowStock = new List<dynamic>
            {
                new { Name = "Tomato", Stock = 3 },
                new { Name = "Apple", Stock = 4 },
                new { Name = "Cashew", Stock = 1 },
                new { Name = "Onion", Stock = 2 },
                new { Name = "JackFruit", Stock = 5 },
                new { Name = "DryFig", Stock = 4 },
            };

            ViewBag.LowStockProducts = lowStock;

            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult EditProfile()
        {
            return View();
        }

        public IActionResult Setting()
        {
            return View();
        }

       

    }
}
