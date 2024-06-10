using BookWebshopEducation.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using BookWebshopEducation.Models.ViewModels;
using System;
using BookWebshopEducation.Models.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BookWebshopEducation.Utility;
using Mono.TextTemplating;
using Stripe.Tax;
using Stripe.Checkout;
using NuGet.ProjectModel;

namespace BookWebshopEducation.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var applicationUser = _unitOfWork.ApplicationUser.Get(user => user.Id == userId);

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(user => user.Id.ToString() == userId, includeProperties: "Product").ToList(),
                OrderHeader = new OrderHeader(),
            };

            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCartList)
            {
                if(shoppingCart.Count <= 50)
                {
                    ShoppingCartViewModel.OrderHeader.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price;
                } 
                else if(shoppingCart.Count > 50 && shoppingCart.Count <= 100)
                {
                    ShoppingCartViewModel.OrderHeader.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price50;
                }
                else if(shoppingCart.Count > 100)
                {
                    ShoppingCartViewModel.OrderHeader.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price100;
                }
            }

            return View(ShoppingCartViewModel);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var applicationUser = _unitOfWork.ApplicationUser.Get(user => user.Id == userId);

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(user => user.Id.ToString() == userId, includeProperties: "Product").ToList(),
                OrderHeader = new OrderHeader(),
            };

            ShoppingCartViewModel.OrderHeader.ApplicationUser = applicationUser;
            ShoppingCartViewModel.OrderHeader.Name = applicationUser.UserName;
            ShoppingCartViewModel.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            ShoppingCartViewModel.OrderHeader.StreetAddress = applicationUser.StreetAddress;
            ShoppingCartViewModel.OrderHeader.City = applicationUser.City;
            ShoppingCartViewModel.OrderHeader.State = applicationUser.Country;
            ShoppingCartViewModel.OrderHeader.PostalCode = applicationUser.PostalCode;

            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCartList)
            {
                if (shoppingCart.Count <= 50)
                {
                    ShoppingCartViewModel.OrderHeader.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price;
                }
                else if (shoppingCart.Count > 50 && shoppingCart.Count <= 100)
                {
                    ShoppingCartViewModel.OrderHeader.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price50;
                }
                else if (shoppingCart.Count > 100)
                {
                    ShoppingCartViewModel.OrderHeader.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price100;
                }
            }

            return View(ShoppingCartViewModel);
        }

        [HttpPost]
        public IActionResult IncrementCount(int productId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.Get(sc => sc.ProductId == productId, includeProperties: "Product");
            shoppingCart.Count++;
            ShoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList();

            if (shoppingCart.Count <= 50)
            {
                ShoppingCartViewModel.OrderHeader.OrderTotal = shoppingCart.Count * shoppingCart.Product.Price;
            }
            else if (shoppingCart.Count > 50 && shoppingCart.Count <= 100)
            {
                ShoppingCartViewModel.OrderHeader.OrderTotal = shoppingCart.Count * shoppingCart.Product.Price50;
            }
            else if (shoppingCart.Count > 100)
            {
                ShoppingCartViewModel.OrderHeader.OrderTotal = shoppingCart.Count * shoppingCart.Product.Price100;
            }

            _unitOfWork.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DecrementCount(int productId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.Get(sc => sc.ProductId == productId, includeProperties: "Product");
            shoppingCart.Count--;
            ShoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList();

            if (shoppingCart.Count < 1)
            {
                _unitOfWork.ShoppingCart.Delete(shoppingCart);
            }

            if (shoppingCart.Count <= 50)
            {
                ShoppingCartViewModel.OrderHeader.OrderTotal = shoppingCart.Count * shoppingCart.Product.Price;
            }
            else if (shoppingCart.Count > 50 && shoppingCart.Count <= 100)
            {
                ShoppingCartViewModel.OrderHeader.OrderTotal = shoppingCart.Count * shoppingCart.Product.Price50;
            }
            else if (shoppingCart.Count > 100)
            {
                ShoppingCartViewModel.OrderHeader.OrderTotal = shoppingCart.Count * shoppingCart.Product.Price100;
            }

            _unitOfWork.SaveChanges();

            return Json(new { success = true });
        }

        [HttpDelete]
        public IActionResult Delete(int shoppingCartId) 
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(sc => sc.Id == shoppingCartId, includeProperties: "Product");

            ShoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(includeProperties: "Product").ToList();

            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCartList)
            {
                if (shoppingCart.Count <= 50)
                {
                    ShoppingCartViewModel.OrderHeader.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price;
                }
                else if (shoppingCart.Count > 50 && shoppingCart.Count <= 100)
                {
                    ShoppingCartViewModel.OrderHeader.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price50;
                }
                else if (shoppingCart.Count > 100)
                {
                    ShoppingCartViewModel.OrderHeader.OrderTotal += shoppingCart.Count * shoppingCart.Product.Price100;
                }
            }

            if (shoppingCartFromDb.Count <= 50)
            {
                ShoppingCartViewModel.OrderHeader.OrderTotal -= shoppingCartFromDb.Count * shoppingCartFromDb.Product.Price;
            }
            else if (shoppingCartFromDb.Count > 50 && shoppingCartFromDb.Count <= 100)
            {
                ShoppingCartViewModel.OrderHeader.OrderTotal -= shoppingCartFromDb.Count * shoppingCartFromDb.Product.Price50;
            }
            else if (shoppingCartFromDb.Count > 100)
            {
                ShoppingCartViewModel.OrderHeader.OrderTotal -= shoppingCartFromDb.Count * shoppingCartFromDb.Product.Price100;
            }
            

            List<ShoppingCart> shoppingCarts = ShoppingCartViewModel.ShoppingCartList.ToList();
            shoppingCarts.Remove(shoppingCartFromDb);

            ShoppingCartViewModel.ShoppingCartList = shoppingCarts.AsEnumerable();

            _unitOfWork.ShoppingCart.Delete(shoppingCartFromDb);
            _unitOfWork.SaveChanges();

            return Json(new { success = true, message = "Item deleted successfully." });
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == userId, includeProperties: "Product");

            ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now.ToUniversalTime();
            ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(user => user.Id == userId);

            /////

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartViewModel.OrderHeader.PaymentStatus = PaymentStatus.Pending;
                ShoppingCartViewModel.OrderHeader.OrderStatus = OrderStatus.Pending;
            }
            else
            {
                ShoppingCartViewModel.OrderHeader.PaymentStatus = PaymentStatus.Delayed;
                ShoppingCartViewModel.OrderHeader.OrderStatus = OrderStatus.Approved;
            }

            _unitOfWork.OrderHeaders.Add(ShoppingCartViewModel.OrderHeader);
            _unitOfWork.SaveChanges();

            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = shoppingCart.ProductId,
                    OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id,
                    Price = shoppingCart.Product.Price * shoppingCart.Count,
                    Count = shoppingCart.Count,
                };
                _unitOfWork.OrderDetails.Add(orderDetail);
                _unitOfWork.SaveChanges();
            }

            /////

            if (applicationUser.CompanyId == null || applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:7088/";

                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/shoppingcart/OrderConfirmation?id={ShoppingCartViewModel.OrderHeader.Id}",
                    CancelUrl = domain + "customer/shoppingcart/Index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartViewModel.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Product.Price * 100),
                            Currency = "eur",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.Product.Title,
                            }
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                };

                var service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeaders.UpdateStripePaymentId(ShoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.SaveChanges();

                Response.Headers.Add("Location", session.Url);

                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaders.Get(oh => oh.Id == id, includeProperties: "ApplicationUser");

            if(orderHeader.PaymentStatus != PaymentStatus.Delayed)
            {
                var sessionService = new SessionService();
                Session session = sessionService.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaders.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeaders.UpdateStatus(id, OrderStatus.Approved, PaymentStatus.Approved);
                    _unitOfWork.SaveChanges();
                }
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.DeleteRange(shoppingCarts);
            _unitOfWork.SaveChanges();

            return View(id);
        }
    }
}
