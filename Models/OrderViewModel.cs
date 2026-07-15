namespace DAShopTech.Models
{
    public class OrderViewModel
    {
        public List<CartItem> CartItems { get; set; } // Danh sách các sản phẩm trong giỏ hàng
        public string CustomerName { get; set; } // Tên khách hàng
        public string PhoneNumber { get; set; } // Số điện thoại
        public string ShippingAddress { get; set; } // Địa chỉ giao hàng
        public string PaymentMethod { get; set; } // Thêm thuộc tính phương thức thanh toá
        public decimal TotalAmount { get; set; } // Tổng số tiền đơn hàng
    }

}
