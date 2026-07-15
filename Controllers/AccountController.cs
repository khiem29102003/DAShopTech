using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DAShopTech.Models; 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims; 

namespace DAShopTech.Controllers
{
    public class AccountController : Controller
    {
        private readonly ShopTechDbContext _context; 

        public AccountController(ShopTechDbContext context)
        {
            _context = context; // Khởi tạo context để kết nối với cơ sở dữ liệu
        }

        // Đăng nhập bằng Google
        [HttpGet]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Đăng nhập bằng Facebook
        [HttpGet]
        public IActionResult LoginWithFacebook()
        {
            var redirectUrl = Url.Action("FacebookResponse", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        // Xử lý phản hồi từ Google
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal != null)
            {
                // Xử lý thông tin từ Google và đăng nhập người dùng
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                var name = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

                // Lưu thông tin người dùng vào session hoặc cơ sở dữ liệu
                HttpContext.Session.SetString("Username", name);
                HttpContext.Session.SetString("Email", email);

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login");
        }

        // Xử lý phản hồi từ Facebook
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal != null)
            {
                // Xử lý thông tin từ Facebook và đăng nhập người dùng
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                var name = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

                // Lưu thông tin người dùng vào session hoặc cơ sở dữ liệu
                HttpContext.Session.SetString("Username", name);
                HttpContext.Session.SetString("Email", email);

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login");
        }

        // GET: Hiển thị trang đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        /*       public IActionResult Login(string username, string password)
               {
                   var hashedPassword = ComputeSha256Hash(password); // Mã hóa mật khẩu người dùng nhập vào
                   var user = _context.Users.SingleOrDefault(u => u.Username == username && u.Password == hashedPassword);

                   if (user != null)
                   {
                       // Lưu thông tin người dùng vào Session
                       HttpContext.Session.SetString("Username", user.Username);
                       HttpContext.Session.SetString("UserType", user.UserType);

                       // Kiểm tra xem người dùng có phải là admin không
                       if (user.UserType == "Admin")
                       {
                           // Tìm admin tương ứng
                           var admin = _context.Admins.SingleOrDefault(a => a.UserId == user.UserId);
                           if (admin != null)
                           {
                               // Lưu AdminId vào session
                               HttpContext.Session.SetInt32("AdminId", admin.AdminId); // Đảm bảo tên đúng
                           }

                           TempData["SuccessMessage"] = "Đăng nhập thành công vào trang Admin!";
                           // Nếu là Admin, điều hướng đến trang Admin
                           return RedirectToAction("Index", "Admin");
                       }
                       else
                       {
                           // Nếu không phải Admin, điều hướng đến trang Home
                           return RedirectToAction("Index", "Home");
                       }
                   }

                   // Nếu đăng nhập không thành công, hiển thị thông báo lỗi
                   ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                   return View();
               }
       */
        public IActionResult Login(string username, string password)
        {
            var hashedPassword = ComputeSha256Hash(password); // Mã hóa mật khẩu người dùng nhập vào
            var user = _context.Users.SingleOrDefault(u => u.Username == username && u.Password == hashedPassword);

            if (user != null)
            {
                // Lưu thông tin người dùng vào Session
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserType", user.UserType);
                HttpContext.Session.SetInt32("UserId", user.UserId); // Lưu ID người dùng vào Session

                // Kiểm tra xem người dùng có phải là admin không
                if (user.UserType == "Admin")
                {
                    // Tìm admin tương ứng
                    var admin = _context.Admins.SingleOrDefault(a => a.UserId == user.UserId);
                    if (admin != null)
                    {
                        // Lưu AdminId vào session
                        HttpContext.Session.SetInt32("AdminId", admin.AdminId); // Đảm bảo tên đúng
                    }

                    TempData["SuccessMessage"] = "Đăng nhập thành công vào trang Admin!";
                    // Nếu là Admin, điều hướng đến trang Admin
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Nếu không phải Admin, điều hướng đến trang Home
                    return RedirectToAction("Index", "Home");
                }
            }

            // Nếu đăng nhập không thành công, hiển thị thông báo lỗi
            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }

        // GET: Hiển thị trang đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Xử lý đăng ký
        [HttpPost]
        public IActionResult Register(string username, string email, string password)
        {
            var hashedPassword = ComputeSha256Hash(password); // Mã hóa mật khẩu người dùng
            // Tạo đối tượng người dùng mới
            var user = new User
            {
                Username = username,
                Email = email,
                Password = hashedPassword,
                UserType = "User" // Mặc định UserType là User
            };

            _context.Users.Add(user); // Thêm người dùng mới vào cơ sở dữ liệu
            _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            // Hiển thị thông báo đăng ký thành công
            ViewBag.Message = "Đăng ký thành công";
            return View();
        }
        [HttpGet]
        public IActionResult CreateAdmin()
        {
            return View();
        } 
        public IActionResult Logout()
        {
            // Xóa thông tin tài khoản khỏi Session
            HttpContext.Session.Clear();
            // Đăng xuất người dùng
            return RedirectToAction("Index", "Home");
        }

        // POST: Xử lý tạo tài khoản admin
        [HttpPost]
        public IActionResult CreateAccounts(string Username, string Password, string Email, string UserType, int AccountStatusId)
        {
            // Mã hóa mật khẩu trước khi lưu vào database
            var hashedPassword = ComputeSha256Hash(Password);

            // Tạo đối tượng người dùng mới
            var newUser = new User
            {
                Username = Username,
                Password = hashedPassword,
                Email = Email,
                UserType = UserType
            };

            // Lưu đối tượng người dùng vào database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Trả về thông báo thành công
            ViewBag.SuccessMessage = "Tài khoản đã được tạo thành công!";
            return View();
        }

        // Hàm mã hóa mật khẩu bằng SHA-256
        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        } 
    }
}
 