using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWebshopEducation.Models.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public string ISBN { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        [DisplayName("List Price")]
        [Range(1D, 1000D)]
        public double ListPrice { get; set; }
        [Range(1D, 1000D)]
        public double Price { get; set; }
        [DisplayName("Price for 50+")]
        public double Price50 { get; set; }
        [DisplayName("Price for 100+")]
        public double Price100 { get; set; }
    }
}
