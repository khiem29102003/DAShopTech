using System;
using System.Collections.Generic;

namespace DAShopTech.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Otp { get; set; } // Thêm thuộc tính OTP

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
