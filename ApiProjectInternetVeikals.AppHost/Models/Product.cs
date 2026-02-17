using ApiProjectInternetVeikals.AppHost.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiProjectInternetVeikals.AppHost.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}