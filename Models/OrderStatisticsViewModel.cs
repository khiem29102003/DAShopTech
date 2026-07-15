using System;
using System.Collections.Generic;
namespace DAShopTech.Models
{
    public class OrderStatisticsViewModel
    {
        public List<Order> Orders { get; set; } = new List<Order>(); // Khởi tạo danh sách rỗng
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public DateTime SelectedDate { get; set; }
        public string Status { get; set; } // Thêm thuộc tính Status
        public int SelectedYear { get; set; } // Thêm thuộc tính SelectedYear 
        public string SelectedStatus { get; set; } // Thêm thuộc tính SelectedStatus
        public List<RevenueDetail> RevenueDetails { get; set; } = new List<RevenueDetail>(); // Khởi tạo danh sách rỗng
        public List<decimal> MonthlyRevenue { get; set; } = new List<decimal>(); // Khởi tạo danh sách rỗng
        public List<decimal> YearlyRevenue { get; set; } = new List<decimal>(); // Khởi tạo danh sách rỗng
    } 
    public class RevenueDetail
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
    }



}
