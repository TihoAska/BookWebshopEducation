using BookWebshopEducation.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookWebshopEducation.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
