using DAShopTech.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using DAShopTech.Models; // Thay đổi theo namespace của bạn

public class ProductController : Controller
{
    private readonly ShopTechDbContext _context;

    public ProductController(ShopTechDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Search(string q)
    {
        ViewData["SearchTerm"] = q;

        var products = _context.Products
            .Where(p => p.ProductName.Contains(q))
            .ToList();

        return View("SearchResults", products);
    }
    [HttpGet]
    public IActionResult SearchSuggestions(string q)
    {
        var suggestions = _context.Products
            .Where(p => p.ProductName.Contains(q))
            .Select(p => new { p.ProductId, p.ProductName })
            .ToList();

        return Json(suggestions);
    }


}
