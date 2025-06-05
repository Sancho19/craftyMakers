﻿namespace TiaPlatform.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public ICollection<OrderItem> Items { get; set; }
    }

}
