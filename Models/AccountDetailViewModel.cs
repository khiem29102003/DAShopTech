using System.Collections.Generic;

namespace DAShopTech.Models
{
    public class AccountDetailViewModel
    {
        public User User { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
