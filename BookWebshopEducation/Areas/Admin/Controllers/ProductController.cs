using BookWebshopEducation.DataAccess.Data;
using BookWebshopEducation.DataAccess.Repository.IRepository;
using BookWebshopEducation.Models.Models;
using BookWebshopEducation.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookWebshopEducation.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll().ToList();
            return View(products);
        }

        public IActionResult Upsert(int? productId)
        {
            IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });
            //ViewBag.CategoryList = categoryList;

            ProductViewModel productViewModel = new ProductViewModel()
            {
                CategoryList = categoryList,
                Product = productId != null ? _unitOfWork.Product.Get(p => p.Id == productId) : new Product(),
            };

            return View(productViewModel);
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productViewModel, IFormFile? formFile)
        {
            Console.WriteLine("CategoryID" + productViewModel.Product.CategoryId);

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if(formFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if(!string.IsNullOrEmpty(productViewModel.Product.ImageUrl))
                    {
                        var oldimagePath = Path.Combine(wwwRootPath, productViewModel.Product.ImageUrl.Trim('\\'));

                        if(System.IO.File.Exists(oldimagePath))
                        {
                            System.IO.File.Delete(oldimagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }

                    productViewModel.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if(productViewModel.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productViewModel.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productViewModel.Product);
                }

                _unitOfWork.SaveChanges();
                //TempData["success"] = "Product created successfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });
            }

            return View(productViewModel);
        }

        public IActionResult Delete(int? productId)
        {
            if (productId is null or 0)
            {
                return NotFound();
            }

            Product? product = _unitOfWork.Product.GetAll().FirstOrDefault(c => c.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? productId)
        {
            Product? product = _unitOfWork.Product.GetAll().FirstOrDefault(c => c.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Delete(product);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Product deleted successfully";

            return RedirectToAction("Index", "Product");
        }
    }
}
