using BookWebshopEducation.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using BookWebshopEducation.Models.ViewModels;
using System;
using BookWebshopEducation.Models.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookWebshopEducation.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel();

            shoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList();

            foreach (var shoppingCart in shoppingCartViewModel.ShoppingCartList)
            {
                if(shoppingCart.Count <= 50)
                {
                    shoppingCartViewModel.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price;
                } 
                else if(shoppingCart.Count > 50 && shoppingCart.Count <= 100)
                {
                    shoppingCartViewModel.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price50;
                }
                else if(shoppingCart.Count > 100)
                {
                    shoppingCartViewModel.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price100;
                }
            }

            return View(shoppingCartViewModel);
        }

        public IActionResult Summary()
        {
            ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel();

            shoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList();

            foreach (var shoppingCart in shoppingCartViewModel.ShoppingCartList)
            {
                if (shoppingCart.Count <= 50)
                {
                    shoppingCartViewModel.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price;
                }
                else if (shoppingCart.Count > 50 && shoppingCart.Count <= 100)
                {
                    shoppingCartViewModel.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price50;
                }
                else if (shoppingCart.Count > 100)
                {
                    shoppingCartViewModel.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price100;
                }
            }

            return View(shoppingCartViewModel);
        }

        [HttpPost]
        public IActionResult IncrementCount(int productId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.Get(sc => sc.ProductId == productId, includeProperties: "Product");
            shoppingCart.Count++;

            if (shoppingCart.Count <= 50)
            {
                ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
                {
                    ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList(),
                    OrderTotal = shoppingCart.Count * shoppingCart.Product.Price,
                };
            }
            else if (shoppingCart.Count > 50 && shoppingCart.Count <= 100)
            {
                ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
                {
                    ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList(),
                    OrderTotal = shoppingCart.Count * shoppingCart.Product.Price50,
                };
            }
            else if (shoppingCart.Count > 100)
            {
                ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
                {
                    ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList(),
                    OrderTotal = shoppingCart.Count * shoppingCart.Product.Price100,
                };
            }

            _unitOfWork.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DecrementCount(int productId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.Get(sc => sc.ProductId == productId, includeProperties: "Product");
            shoppingCart.Count--;

            if(shoppingCart.Count < 1)
            {
                _unitOfWork.ShoppingCart.Delete(shoppingCart);
            }

            if (shoppingCart.Count <= 50)
            {
                ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
                {
                    ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList(),
                    OrderTotal = shoppingCart.Count * shoppingCart.Product.Price,
                };
            }
            else if (shoppingCart.Count > 50 && shoppingCart.Count <= 100)
            {
                ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
                {
                    ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList(),
                    OrderTotal = shoppingCart.Count * shoppingCart.Product.Price50,
                };
            }
            else if (shoppingCart.Count > 100)
            {
                ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
                {
                    ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList(),
                    OrderTotal = shoppingCart.Count * shoppingCart.Product.Price100,
                };
            }

            _unitOfWork.SaveChanges();

            return Json(new { success = true });
        }

        [HttpDelete]
        public IActionResult Delete(int shoppingCartId) 
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(sc => sc.Id == shoppingCartId, includeProperties: "Product");

            ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList(),
            };

            foreach (var shoppingCart in shoppingCartViewModel.ShoppingCartList)
            {
                if (shoppingCart.Count <= 50)
                {
                    shoppingCartViewModel.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price;
                }
                else if (shoppingCart.Count > 50 && shoppingCart.Count <= 100)
                {
                    shoppingCartViewModel.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price50;
                }
                else if (shoppingCart.Count > 100)
                {
                    shoppingCartViewModel.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price100;
                }
            }

            if (shoppingCartFromDb.Count <= 50)
            {
                shoppingCartViewModel.OrderTotal -= shoppingCartFromDb.Count * shoppingCartFromDb.Product.Price;
            }
            else if (shoppingCartFromDb.Count > 50 && shoppingCartFromDb.Count <= 100)
            {
                shoppingCartViewModel.OrderTotal -= shoppingCartFromDb.Count * shoppingCartFromDb.Product.Price50;
            }
            else if (shoppingCartFromDb.Count > 100)
            {
                shoppingCartViewModel.OrderTotal -= shoppingCartFromDb.Count * shoppingCartFromDb.Product.Price100;
            }
            

            List<ShoppingCart> shoppingCarts = shoppingCartViewModel.ShoppingCartList.ToList();
            shoppingCarts.Remove(shoppingCartFromDb);

            shoppingCartViewModel.ShoppingCartList = shoppingCarts.AsEnumerable();

            _unitOfWork.ShoppingCart.Delete(shoppingCartFromDb);
            _unitOfWork.SaveChanges();

            return Json(new { success = true, message = "Item deleted successfully." });
        }
    }
}
