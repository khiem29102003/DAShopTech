using System;
using System.Collections.Generic;

namespace DAShopTech.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }
    public DateTime? ShippingDate { get; set; }  // Thêm thuộc tính ShippingDate
    public string? ApprovedBy { get; set; }

    public string? ShippingAddress { get; set; }
    public string PaymentMethod { get; set; } // Thêm thuộc tính này
    public string? Notes { get; set; } 
    public int? ConfirmedByUserId { get; set; } 
    public virtual User? ConfirmedByUser { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
