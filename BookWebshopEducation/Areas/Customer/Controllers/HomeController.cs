using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookWebshopEducation.Models.Models;
using BookWebshopEducation.DataAccess.Repository.IRepository;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;
using BookWebshopEducation.Models.ViewModels;

namespace BookWebshopEducation.Areas.Customer.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private static int totalCount = 0;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
        return View(productList);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Details(int productId)
    {
        ShoppingCart shoppingCart = new()
        {
            Product = _unitOfWork.Product.Get(p => p.Id == productId, includeProperties: "Category"),
            Count = 1,
            ProductId = productId,
        };

        return View(shoppingCart);
    }

    [HttpPost]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        shoppingCart.ApplicationUserId = userId;

        ShoppingCart shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(sp => sp.ApplicationUserId == userId && sp.ProductId == shoppingCart.ProductId);

        if(shoppingCartFromDb != null)
        {
            shoppingCartFromDb.Count += shoppingCart.Count;
            _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
        }
        else
        {
            _unitOfWork.ShoppingCart.Add(shoppingCart);
        }

        _unitOfWork.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult GetCartCount()
    {
        var totalCount = _unitOfWork.ShoppingCart.GetAll().Sum(item => item.Count);
        return Ok(new { Count = totalCount });
    }
}
