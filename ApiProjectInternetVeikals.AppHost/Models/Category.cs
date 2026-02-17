using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiProjectInternetVeikals.AppHost.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}