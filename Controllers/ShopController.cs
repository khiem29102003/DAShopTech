using DAShopTech.Models;
using Microsoft.AspNetCore.Mvc;

public class ShopController : Controller
{
    private readonly ShopTechDbContext _context;

    public ShopController(ShopTechDbContext context)
    {
        _context = context;
    }

    // Hiển thị sản phẩm theo CategoryId
    public IActionResult Index(int? categoryId)
    {
        // Lấy toàn bộ danh mục
        var categories = _context.Categories.ToList();

        // Nếu categoryId không có giá trị, lấy toàn bộ sản phẩm
        var products = _context.Products.ToList();

        // Nếu có categoryId, lọc sản phẩm theo danh mục đó
        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
        }

        // Tạo ViewModel chứa sản phẩm và danh mục
        var viewModel = new ShopViewModel
        {
            Products = products,
            Categories = categories
        };

        // Truyền ViewModel sang View
        return View(viewModel);
    }

    // Chi tiết sản phẩm
    [HttpGet("Shop/ProductDetail/{id}")]
    public IActionResult ProductDetail(int id)
    {
        var productDetail = _context.Products.FirstOrDefault(p => p.ProductId == id);
        if (productDetail == null)
        {
            return NotFound();
        }
        return View(productDetail);
    }
}
