using System.Collections.Generic;
using DAShopTech.Models;

namespace DAShopTech.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ShippingAddress { get; set; }
        public string Notes { get; set; }
        // Các thuộc tính khác nếu cần
    }

}
