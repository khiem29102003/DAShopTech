using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAShopTech.Models;

public partial class Product
{
    public int ProductId { get; set; }

    [Display(Name = "Tên sản phẩm")]
    public string ProductName { get; set; } = null!;

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Giá bán")]
    public decimal Price { get; set; }

    [Display(Name = "Hãng sản xuất")]
    public string? Manufacturer { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Nhà cung cấp")]
    public string? Supplier { get; set; }

    [Display(Name = "Giá nhập")]
    public decimal? PurchasePrice { get; set; }

    [Display(Name = "Số lượng")]
    public int? Quantity { get; set; }

    [Display(Name = "Người nhập")]
    public string? EnteredBy { get; set; }

    [Display(Name = "Loại sản phẩm")]
    public int? CategoryId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [Display(Name = "Liên kết hình ảnh sản phẩm")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Phần trăm giảm giá")]
    public decimal? DiscountPercent { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
