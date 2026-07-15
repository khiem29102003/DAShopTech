using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DAShopTech.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAShopTech.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartCountViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke()
        {
            var cart = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var itemCount = cart.Sum(item => item.Quantity);
            return Content(itemCount.ToString());
        }
    }
}
