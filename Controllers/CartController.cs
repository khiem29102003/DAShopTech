using Microsoft.AspNetCore.Mvc;
using DAShopTech.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace DAShopTech.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopTechDbContext _context;
        private const string CartSessionKey = "CartItems";

        public CartController(ShopTechDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            List<CartItem> cartItems = GetCartItems();

            var cartItem = cartItems.FirstOrDefault(c => c.Product.ProductId == id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cartItems.Add(new CartItem { Product = product, Quantity = 1 });
            }

            SaveCartItems(cartItems);

            TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng thành công!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            List<CartItem> cartItems = GetCartItems();
            ViewBag.Message = TempData["Message"];
            return View(cartItems);
        }

        private List<CartItem> GetCartItems()
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey);
            return cartItems ?? new List<CartItem>();
        }

        private void SaveCartItems(List<CartItem> cartItems)
        {
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cartItems);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            List<CartItem> cartItems = GetCartItems();
            var cartItem = cartItems.FirstOrDefault(c => c.Product.ProductId == productId);
            if (cartItem != null)
            {
                cartItems.Remove(cartItem);
                SaveCartItems(cartItems);
            }

            ViewBag.Message = "Sản phẩm đã được xóa khỏi giỏ hàng!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateCart([FromBody] List<CartItemUpdateModel> cartItems)
        {
            if (cartItems == null)
            {
                return BadRequest();
            }

            var sessionCartItems = GetCartItems();

            foreach (var item in cartItems)
            {
                var sessionItem = sessionCartItems.FirstOrDefault(c => c.Product.ProductId == item.ProductId);
                if (sessionItem != null)
                {
                    sessionItem.Quantity = item.Quantity;
                }
            }

            SaveCartItems(sessionCartItems);
            return Ok();
        }

        [HttpGet]
        public JsonResult GetCartCount()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            var itemCount = cart.Sum(item => item.Quantity);

            return Json(itemCount);
        }

        public class CartItemUpdateModel
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
