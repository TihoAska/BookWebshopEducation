using System;
using System.Collections.Generic;
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
        public string Name { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime YearOfPublish { get; set; }
        public Category? Category { get; set; }
        public decimal Price { get; set; }
        public string Picture { get; set; } = string.Empty;
        public int NumberOfPages { get; set; }
    }
}
