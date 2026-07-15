using DAShopTech.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class DataInitializer
{
    public static void Initialize(ShopTechDbContext context)
    {
        // Kiểm tra xem có dữ liệu nào chưa
        bool hasData = context.Users.Any() || context.Products.Any() || context.Categories.Any();
        if (hasData)
        {
            return; // Nếu đã có dữ liệu thì không làm gì cả
        }

        // Thêm tài khoản admin
        var adminUser = new User
        {
            Username = "admin2",
            Email = "admin@example.com",
            Password = ComputeSha256Hash("adminpassword"), // Mã hóa mật khẩu
            UserType = "Admin"
        };
        context.Users.Add(adminUser);

        // Thêm danh mục sản phẩm mẫu
        var categories = new List<Category>
        {
            new Category { CategoryName = "Laptop" },
            new Category { CategoryName = "Điện thoại" },
            new Category { CategoryName = "Phụ kiện" }
        };
        context.Categories.AddRange(categories);
        context.SaveChanges();

        // Lấy lại CategoryId sau khi SaveChanges
        var laptopCategory = context.Categories.FirstOrDefault(c => c.CategoryName == "Laptop");
        var phoneCategory = context.Categories.FirstOrDefault(c => c.CategoryName == "Điện thoại");
        var accessoryCategory = context.Categories.FirstOrDefault(c => c.CategoryName == "Phụ kiện");

        // Thêm sản phẩm mẫu
        var products = new List<Product>
        {
            new Product {
                ProductName = "Laptop ASUS VivoBook",
                Description = "Laptop ASUS VivoBook 2024, Intel Core i5, RAM 8GB, SSD 512GB",
                Price = 15990000,
                Manufacturer = "ASUS",
                Status = "Còn hàng",
                Supplier = "ASUS Việt Nam",
                PurchasePrice = 14000000,
                Quantity = 10,
                EnteredBy = "admin2",
                CategoryId = laptopCategory?.CategoryId
            },
            new Product {
                ProductName = "iPhone 15 Pro Max",
                Description = "Apple iPhone 15 Pro Max 256GB, màu Titan Xanh",
                Price = 29990000,
                Manufacturer = "Apple",
                Status = "Còn hàng",
                Supplier = "Apple Việt Nam",
                PurchasePrice = 27000000,
                Quantity = 15,
                EnteredBy = "admin2",
                CategoryId = phoneCategory?.CategoryId
            },
            new Product {
                ProductName = "Tai nghe Bluetooth Sony WF-1000XM4",
                Description = "Tai nghe chống ồn, pin 24h, Bluetooth 5.2",
                Price = 4990000,
                Manufacturer = "Sony",
                Status = "Còn hàng",
                Supplier = "Sony Việt Nam",
                PurchasePrice = 4200000,
                Quantity = 30,
                EnteredBy = "admin2",
                CategoryId = accessoryCategory?.CategoryId
            }
        };
        context.Products.AddRange(products);
        context.SaveChanges();
    }

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
