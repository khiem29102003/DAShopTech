using Microsoft.AspNetCore.Mvc;
using DAShopTech.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Mail;

namespace DAShopTech.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly ShopTechDbContext _context;
        private const string CartSessionKey = "CartItems";

        public CheckOutController(ShopTechDbContext context)
        {
            _context = context;
        }

        // Hiển thị trang Checkout với thông tin giỏ hàng
        public IActionResult Index()
        {
            var cartItems = GetCartItems();
            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            var totalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity);

            var viewModel = new OrderViewModel
            {
                CartItems = cartItems,
                TotalAmount = totalAmount
            };

            return View(viewModel); // Truyền model đến view
        }

        /*  [HttpPost]
          public IActionResult ConfirmOrder(string customerName, string phoneNumber, string shippingAddress, string paymentMethod)
          {
              var cartItems = GetCartItems();
              if (!cartItems.Any())
              {
                  return RedirectToAction("Index", "Home");
              }

              int customerId;

              // Kiểm tra xem người dùng có đăng nhập hay không
              var userId = HttpContext.Session.GetInt32("UserId"); // Giả sử UserId được lưu trong session khi đăng nhập
              if (userId.HasValue)
              {
                  // Nếu người dùng đã đăng nhập, lấy CustomerId từ hệ thống
                  var customer = _context.Customers.FirstOrDefault(c => c.UserId == userId.Value);
                  if (customer != null)
                  {
                      customerId = customer.CustomerId;
                  }
                  else
                  {
                      // Xử lý trường hợp không tìm thấy khách hàng tương ứng (có thể hiếm khi xảy ra)
                      return RedirectToAction("Index", "Home");
                  }
              }
              else
              {
                  // Nếu người dùng không đăng nhập, tạo khách hàng mới
                  var newCustomer = new Customer
                  {
                      FullName = customerName,
                      Phone = phoneNumber,
                      Address = shippingAddress,
                      // Thiết lập giá trị mặc định cho CustomerStatus và CustomerType
                      CustomerStatus = "Active", // Giá trị mặc định, có thể thay đổi theo yêu cầu
                      CustomerType = "Regular" // Giá trị mặc định cho CustomerType, có thể thay đổi theo yêu cầu
                                               // Các thuộc tính khác nếu có
                  };
                  _context.Customers.Add(newCustomer);
                  _context.SaveChanges();
                  customerId = newCustomer.CustomerId;
              }

              var order = new Order
              {
                  CustomerId = customerId,
                  OrderDate = DateTime.Now,
                  TotalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity),
                  Status = "Chờ xác nhận",
                  ShippingAddress = shippingAddress, 
              };

              _context.Orders.Add(order);
              _context.SaveChanges();

              foreach (var item in cartItems)
              {
                  var orderDetail = new OrderDetail
                  {
                      OrderId = order.OrderId,
                      ProductId = item.Product.ProductId,
                      Quantity = item.Quantity,
                      UnitPrice = item.Product.Price
                  };
                  _context.OrderDetails.Add(orderDetail);
              }
              _context.SaveChanges();

              // Xóa giỏ hàng sau khi hoàn tất đơn hàng
              HttpContext.Session.Remove(CartSessionKey);

              return RedirectToAction("OrderSuccess", new { orderId = order.OrderId });
          }
  */
        [HttpPost]
        public IActionResult ConfirmOrder(string customerName, string phoneNumber, string shippingAddress, string email, string paymentMethod)
        {
            var cartItems = GetCartItems();
            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            int customerId;
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var customer = _context.Customers.FirstOrDefault(c => c.UserId == userId.Value);
                if (customer != null)
                {
                    customerId = customer.CustomerId;
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                var newCustomer = new Customer
                {
                    FullName = customerName,
                    Phone = phoneNumber,
                    Address = shippingAddress,
                    CustomerStatus = "Active",
                    CustomerType = "Regular"
                };
                _context.Customers.Add(newCustomer);
                _context.SaveChanges();
                customerId = newCustomer.CustomerId;
            }

            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity),
                Status = "Chờ xác nhận",
                ShippingAddress = shippingAddress,
                PaymentMethod = paymentMethod // Thêm dòng này để gán giá trị cho PaymentMethod
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.Product.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();

            // Gửi email xác nhận đơn hàng
            SendConfirmationEmail(email, order);

            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction("OrderSuccess", new { orderId = order.OrderId });
        }

        // Gửi email xác nhận đơn hàng
        private void SendConfirmationEmail(string email, Order order)
        {
            var fromAddress = new MailAddress("antran1137@gmail.com", "ShopTechQA");
            var toAddress = new MailAddress(email);
            const string fromPassword = "an_1105_2003"; // Sử dụng mật khẩu ứng dụng thay vì mật khẩu chính
            string subject = "Xác nhận đơn hàng #" + order.OrderId;
            string body = $"Xin chào {order.Customer.FullName},\n\n"
                        + "Cảm ơn bạn đã đặt hàng tại cửa hàng chúng tôi.\n"
                        + "Vui lòng xác nhận đơn hàng qua liên kết dưới đây:\n"
                        + $"https://your-website.com/confirm-order?orderId={order.OrderId}\n\n"
                        + "Thông tin đơn hàng của bạn:\n"
                        + $"Tổng tiền: {order.TotalAmount.ToString("C", new System.Globalization.CultureInfo("vi-VN"))}\n"
                        + "Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword) // Sử dụng mật khẩu ứng dụng
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                try
                {
                    smtp.Send(message);
                }
                catch (SmtpException ex)
                {
                    // Xử lý lỗi gửi email
                    Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                    // Bạn có thể ghi lại lỗi vào log hoặc thông báo cho người quản trị
                }
            }
        }


        // Xác nhận đơn hàng qua email
        public IActionResult ConfirmEmailOrder(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = "Đã xác nhận";
            _context.SaveChanges();

            return View("OrderConfirmed", order);
        }
        // Hiển thị thành công sau khi đặt hàng
        public IActionResult OrderSuccess(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        // Lấy danh sách các sản phẩm trong giỏ hàng từ session
        private List<CartItem> GetCartItems()
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey);
            return cartItems ?? new List<CartItem>();
        }
    }

}
