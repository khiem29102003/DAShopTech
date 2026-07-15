using DAShopTech.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
namespace DAShopTech.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopTechDbContext _db;
        public HomeController(ShopTechDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            var product = _db.Products.ToList();
            // Lấy thông tin người dùng từ Session
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.UserType = HttpContext.Session.GetString("UserType");
            return View(product);
        } 

    }
}
