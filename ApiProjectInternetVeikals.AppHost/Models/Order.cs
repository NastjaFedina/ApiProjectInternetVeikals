using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiProjectInternetVeikals.AppHost.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }

        public User? User { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}