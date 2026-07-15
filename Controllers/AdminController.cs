using DAShopTech.Models;
using DAShopTech.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace DAShopTech.Controllers
{
    public class AdminController : Controller
    {
        private readonly ShopTechDbContext _context; 
        public AdminController(ShopTechDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /*       public IActionResult Index()
               {
                   // Lấy thông tin người dùng từ session
                   var userId = HttpContext.Session.GetInt32("UserId");

                   if (userId == null)
                   {
                       TempData["ErrorMessage"] = "Bạn cần đăng nhập để truy cập trang này.";
                       return RedirectToAction("Login", "Account");
                   }

                   var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

                   if (user == null)
                   {
                       TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                       return RedirectToAction("Login", "Account");
                   }

                   ViewBag.Username = user.Username;
                   ViewBag.UserRole = user.UserType; // Ví dụ, "Admin"

                   return View();
               }*/
        public IActionResult Index(DateTime? selectedDate, string status)
        {
            // Lấy thông tin người dùng từ session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để truy cập trang này.";
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Username = user.Username;
            ViewBag.UserRole = user.UserType; // Ví dụ, "Admin"

            // Nếu không có ngày được chọn, sử dụng ngày hiện tại
            var today = selectedDate ?? DateTime.Today;

            // Lọc đơn hàng theo trạng thái nếu có
            var ordersQuery = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                ordersQuery = ordersQuery.Where(o => o.Status == status);
            }

            // Lọc đơn hàng theo ngày
            var ordersInDay = ordersQuery
                .Where(o => o.OrderDate.Date == today.Date)
                .ToList();

            var totalRevenue = ordersInDay.Sum(o => o.TotalAmount);
            var totalOrders = ordersInDay.Count;

            var model = new OrderStatisticsViewModel
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                Orders = ordersInDay,
                RevenueDetails = ordersInDay.Select(o => new RevenueDetail
                {
                    OrderId = o.OrderId,
                    Status = o.Status,
                    Amount = o.TotalAmount
                }).ToList(),
                SelectedDate = today,
                MonthlyRevenue = GetMonthlyRevenue(today.Year, today.Month),
                YearlyRevenue = GetYearlyRevenue(today.Year)
            };

            return View(model);
        }

        public List<decimal> GetMonthlyRevenue(int year, int month)
        {
            return _context.Orders
                .Where(o => o.OrderDate.Year == year && o.OrderDate.Month == month)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => g.Sum(o => o.TotalAmount))
                .ToList();
        }

        public List<decimal> GetYearlyRevenue(int year)
        {
            return _context.Orders
                .Where(o => o.OrderDate.Year == year)
                .GroupBy(o => o.OrderDate.Year)
                .Select(g => g.Sum(o => o.TotalAmount))
                .ToList();
        }


        private void CalculateMonthlyRevenue(int year, ref List<decimal> monthlyRevenue)
        {
            // Implement your logic to fill monthlyRevenue list for the given year
        }

        private void CalculateYearlyRevenue(ref List<decimal> yearlyRevenue)
        {
            // Implement your logic to fill yearlyRevenue list
        }

        public IActionResult Logout()
        {
            // Xử lý đăng xuất
            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult Profile()
        {
            // Trả về view thông tin cá nhân
            return View();
        }

        public IActionResult Settings()
        {
            // Trả về view cài đặt
            return View();
        }

        public IActionResult Balance()
        {
            // Trả về view số dư
            return View();
        }
        public IActionResult ProductList()
        {
            /*            // Lấy danh sách sản phẩm cùng với loại sản phẩm (eager loading)
                        var products = _context.Products
                            .Include(p => p.Category) // Nạp thuộc tính Category
                            .ToList(); // Lấy toàn bộ sản phẩm từ cơ sở dữ liệu

                        // Truyền dữ liệu sản phẩm đến view
                        return View(products);*/
            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }
        // Hiển thị form thêm sản phẩm
        public IActionResult CreateProduct()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        } 
        // Xử lý thêm sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(ProductList));
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        public IActionResult AddCategory(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                var newCategory = new Category
                {
                    CategoryName = categoryName
                };

                _context.Categories.Add(newCategory);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Loại sản phẩm mới đã được thêm thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Tên loại sản phẩm không được để trống.";
            }

            return RedirectToAction("ProductList"); // Hoặc Action nào đó mà bạn muốn
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new EditProductViewModel
            {
                Product = product,
                Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList() // Convert to List<SelectListItem>
            };

            return View(viewModel);
        } 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(EditProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = _context.Products.Find(viewModel.Product.ProductId);

                if (product != null)
                {
                    product.ProductName = viewModel.Product.ProductName;
                    product.CategoryId = viewModel.Product.CategoryId;
                    product.Status = viewModel.Product.Status;
                    product.Manufacturer = viewModel.Product.Manufacturer;
                    product.ImageUrl = viewModel.Product.ImageUrl;
                    product.PurchasePrice = viewModel.Product.PurchasePrice;
                    product.Price = viewModel.Product.Price;
                    product.Quantity = viewModel.Product.Quantity;
                    product.EnteredBy = viewModel.Product.EnteredBy;
                    product.UpdatedDate = DateTime.Now;

                    _context.Update(product);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Sản phẩm đã được cập nhật thành công!";
                    return RedirectToAction(nameof(ProductList));
                }

                return NotFound();
            }

            viewModel.Categories = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int productId)
        {
            var product = _context.Products.Find(productId);

            // Ghi log ID sản phẩm và trạng thái tìm thấy
            Console.WriteLine($"Trying to delete product with ID: {productId}");
            Console.WriteLine($"Product found: {product != null}");

            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
            }

            return RedirectToAction(nameof(ProductList));
        }


        public IActionResult ProductsInventory()
        {
            // Tính toán ngày 3 tháng trước
            var threeMonthsAgo = DateTime.Now.AddMonths(-3);

            // Lấy danh sách sản phẩm từ cơ sở dữ liệu
            var products = _context.Products
                .Where(p => p.CreatedDate >= threeMonthsAgo) // Lọc sản phẩm theo ngày thêm vào
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.ImageUrl,
                    CategoryName = p.Category.CategoryName,
                    p.Manufacturer,
                    p.CreatedDate,
                    p.Quantity,
                    p.Supplier,
                    Status = p.Status == "1" ? "Hoạt Động" : "Ngừng Hoạt Động"
                })
                .ToList();

            return View(products);
        }
        [HttpGet]
        public IActionResult ProductsByDate(DateTime date)
        {
            // Lấy danh sách sản phẩm theo ngày được chọn
            var products = _context.Products
                .Where(p => p.CreatedDate.Date == date.Date) // Lọc sản phẩm theo ngày
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.ImageUrl,
                    CategoryName = p.Category.CategoryName,
                    p.Manufacturer,
                    CreatedDate = p.CreatedDate.ToString("dd/MM/yyyy"),
                    p.Quantity,
                    p.Supplier,
                    Status = p.Status == "1" ? "Hoạt Động" : "Ngừng Hoạt Động"
                })
                .ToList();

            return Json(products);
        }
        public IActionResult ProductsNew()
        {
            // Lấy các sản phẩm được thêm vào trong vòng 10 ngày qua và có trạng thái hoạt động (Status = 1)
            var tenDaysAgo = DateTime.Now.AddDays(-10);

            var newProducts = _context.Products
                .Include(p => p.Category) // Bao gồm cả loại sản phẩm
                .Where(p => p.CreatedDate >= tenDaysAgo && p.Status == "1") // Kiểm tra trạng thái hoạt động (Status = 1)
                .ToList();

            return View(newProducts);
        } 
        // Phương thức hiển thị danh sách đơn hàng
        public IActionResult Orders()
        {
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.ConfirmedByUser)  // Include thông tin người xác nhận đơn hàng
                .ToList();

            return View(orders);
        } 
        // Phương thức hiển thị chi tiết đơn hàng
        public IActionResult OrderDetails(int id)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng.";
                return RedirectToAction("Orders");
            }

            return View(order);
        }
        // Phương thức hiển thị đơn hàng mới
        public IActionResult OrdersNews()
        {
            // Lấy các đơn hàng đã xác nhận từ cơ sở dữ liệu
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.Status == "Đã Xác Nhận")
                .ToList();

            return View(orders);
        }
        // Phương thức hiển thị đơn hàng hủy
        public IActionResult OrdersCancel()
        {
            // Lấy các đơn hàng đã hủy từ cơ sở dữ liệu
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.Status == "Đã Hủy")
                .ToList();

            return View(orders);
        }
        // Hiển thị danh sách đơn hàng chờ xác nhận
        public IActionResult OrderAwaitingConfirmation()
        {
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.Status == "Chờ Xác Nhận")
                .ToList();

            return View(orders);
        }
        // Hiển thị danh sách đơn hàng đang giao
        public IActionResult OrdersTransacting()
        {
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.Status == "Đang Giao")
                .ToList();

            var currentDate = DateTime.Now;

            foreach (var order in orders)
            {
                // Kiểm tra nếu đơn hàng đã "Đang Giao" hơn 3 ngày
                if (order.ShippingDate.HasValue && order.ShippingDate.Value.AddSeconds(3) <= currentDate)
                {
                    order.Status = "Giao Thành Công";
                    _context.Orders.Update(order);

                    // Cập nhật số lượng sản phẩm trong kho
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = _context.Products.Find(detail.ProductId);
                        if (product != null)
                        {
                            product.Quantity -= detail.Quantity;
                            _context.Products.Update(product);
                        }
                    }
                }
            }
                
            _context.SaveChanges();
            return View(orders);
        }
        // Hiển thị danh sách đơn hàng giao thành công
        public IActionResult OrdersComplete()
        {
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.Status == "Giao Thành Công")
                .ToList();

            return View(orders);
        }
        //chuyển trạng thái sang xác nhận đơn hàng
        [HttpPost]
        public IActionResult ConfirmOrder(int id)
        {
            // Tìm đơn hàng theo ID
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("OrderAwaitingConfirmation");
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = "Đã Xác Nhận";
            order.ConfirmedByUserId = HttpContext.Session.GetInt32("UserId"); // Gán người xác nhận

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đơn hàng đã được xác nhận thành công.";
            return RedirectToAction("OrderAwaitingConfirmation");
        }
        //chuyển trạng thái sang giao hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkOrderAsTransacting(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra trạng thái hiện tại để đảm bảo chỉ thay đổi từ "Đã Xác Nhận"
            if (order.Status == "Đã Xác Nhận")
            {
                order.Status = "Đang Giao";
                order.ShippingDate = DateTime.Now;  // Gán ngày giao hàng
                _context.SaveChanges();
            }

            return RedirectToAction("OrdersTransacting");
        }

        //chuyển trạng thái giao thành công
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrderSuccessfully(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra trạng thái hiện tại để đảm bảo chỉ thay đổi từ "Đang Giao"
            if (order.Status == "Đang Giao")
            {
                order.Status = "Giao Thành Công";
                _context.SaveChanges();
            }

            return RedirectToAction("OrdersTransacting");
        }

 

        //chuyển trạng thái hủy đơn
        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            // Tìm đơn hàng theo ID
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("OrderAwaitingConfirmation");
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = "Đã Hủy";
            order.ConfirmedByUserId = HttpContext.Session.GetInt32("UserId"); // Gán người hủy (nếu cần)

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công.";
            return RedirectToAction("OrderAwaitingConfirmation");
        } 
        public IActionResult Customer()
        {
            // Lấy danh sách khách hàng đã mua hàng bằng cách kiểm tra các đơn hàng đã tồn tại
            var customers = _context.Orders
                .Include(o => o.Customer) // Bao gồm thông tin khách hàng
                .Select(o => o.Customer)  // Lấy ra thông tin khách hàng
                .Distinct() // Đảm bảo không trùng lặp khách hàng
                .ToList();

            return View(customers);
        }
        public IActionResult CustomerNew()
        {
            // Lấy ngày hiện tại (chỉ phần ngày, không bao gồm giờ)
            var today = DateTime.Today;

            // Lấy danh sách khách hàng đã mua hàng trong ngày hôm nay
            var newCustomers = _context.Orders
                .Include(o => o.Customer) // Bao gồm thông tin khách hàng
                .Where(o => o.OrderDate >= today && o.OrderDate < today.AddDays(1)) // Đơn hàng trong ngày hôm nay
                .Select(o => o.Customer)  // Lấy ra thông tin khách hàng
                .Distinct() // Đảm bảo không trùng lặp khách hàng
                .ToList();

            return View(newCustomers);
        }
        public IActionResult PotentialCustomers()
        {
            return View();
        }
        public IActionResult LoyalCustomers()
        {
            return View();
        }
        public IActionResult Accounts()
        {
            // Lọc người dùng theo UserType
            var users = _context.Users
                .Where(u => u.UserType == "User")
                .ToList();

            return View(users);
        }
        [HttpPost]
        public IActionResult CreateAccounts(User model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tài khoản đã tồn tại hay chưa
                var existingUser = _context.Users.FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ViewBag.ErrorMessage = "Tài khoản đã tồn tại!";
                    return View();
                }

                // Mã hóa mật khẩu
                model.Password = ComputeSha256Hash(model.Password);

                // Thêm tài khoản vào cơ sở dữ liệu
                _context.Users.Add(model);
                _context.SaveChanges();

                // Thông báo thành công
                ViewBag.SuccessMessage = "Tài khoản đã được tạo thành công!";
                return RedirectToAction("AccountsList");
            }

            return View(model);
        }
        public string ComputeSha256Hash(string rawData)
        {
            // Tạo một thể hiện của SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Chuyển đổi chuỗi thành mảng byte
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Chuyển đổi mảng byte thành chuỗi
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        // Phương thức hiển thị trang xác nhận xóa tài khoản
        public IActionResult ConfirmDeleteAccount(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản để xóa.";
                return RedirectToAction("Accounts");
            }

            return View(user);
        }

        // Phương thức xử lý xóa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAccount(int id)
        {
            var user = _context.Users
                .Include(u => u.Customers)
                .ThenInclude(c => c.Orders)
                .ThenInclude(o => o.OrderDetails)
                .FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản để xóa.";
                return RedirectToAction("Accounts");
            }

            // Xóa tất cả các chi tiết đơn hàng liên quan đến đơn hàng của khách hàng
            foreach (var customer in user.Customers)
            {
                foreach (var order in customer.Orders)
                {
                    var orderDetails = _context.OrderDetails.Where(od => od.OrderId == order.OrderId).ToList();
                    foreach (var orderDetail in orderDetails)
                    {
                        _context.OrderDetails.Remove(orderDetail);
                    }
                }
            }

            // Xóa tất cả các đơn hàng liên quan đến khách hàng của người dùng
            foreach (var customer in user.Customers)
            {
                var orders = _context.Orders.Where(o => o.CustomerId == customer.CustomerId).ToList();
                foreach (var order in orders)
                {
                    _context.Orders.Remove(order);
                }
            }

            // Xóa tất cả các khách hàng liên quan đến người dùng
            foreach (var customer in user.Customers.ToList())
            {
                _context.Customers.Remove(customer);
            }

            // Xóa người dùng
            _context.Users.Remove(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Tài khoản đã được xóa thành công.";
            return RedirectToAction("Accounts");
        }
        // Phương thức để hiển thị thông tin tài khoản và các đơn hàng đã mua
        public IActionResult AccountDetail(int id)
        {
            // Lấy thông tin tài khoản, khách hàng, đơn hàng và chi tiết đơn hàng
            var user = _context.Users
                .Include(u => u.Customers)
                    .ThenInclude(c => c.Orders)
                        .ThenInclude(o => o.OrderDetails)
                            .ThenInclude(od => od.Product)
                .FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("Accounts");
            }
             
            var customer = user.Customers.FirstOrDefault();
            var orders = customer?.Orders.ToList() ?? new List<Order>();

            var viewModel = new AccountDetailViewModel
            {
                User = user,
                Orders = orders
            };

            return View(viewModel);
        }
    }
}
 