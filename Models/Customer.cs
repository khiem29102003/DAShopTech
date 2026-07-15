using System;
using System.Collections.Generic;

namespace DAShopTech.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public int? UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string CustomerType { get; set; } = null!;

    public string CustomerStatus { get; set; } = null!;

    public decimal? TotalSpent { get; set; }

    public DateTime RegistrationDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? User { get; set; } 
}
