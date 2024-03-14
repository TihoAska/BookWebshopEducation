using Microsoft.AspNetCore.Mvc;
using BookWebshopEducation.DataAccess.Data;
using BookWebshopEducation.Models.Models;
using BookWebshopEducation.DataAccess.Repository.IRepository;
using BookWebshopEducation.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookWebshopEducation.Controllers;

public class CategoryController : Controller
{
    //private readonly ICategoryRepository _categoryRepository;
    //private readonly ApplicationDbContext _context;
    private IUnitOfWork _unitOfWork;

    //public CategoryController(ICategoryRepository categoryRepository, ApplicationDbContext contex)
    //{
    //    _categoryRepository = categoryRepository;
    //    _context = contex;
    //}

    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        List<Category> categoryList = _unitOfWork.Category.GetAll().ToList();
        return View(categoryList);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        //Custom validation
        if (category.Name == category.DisplayOrder.ToString())
        {
            ModelState.AddModelError("Name", "Name can't be the same as DisplayOrder.");
        }

        if (ModelState.IsValid) 
        {
            _unitOfWork.Category.Add(category);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Category created successfully";
            return RedirectToAction("Index", "Category");
        }

        return View();
    }

    public IActionResult Edit(int? categoryId)
    {
        if (categoryId is null or 0)
        {
            return NotFound();
        }

        Category? category = _unitOfWork.Category.Get(c => c.Id == categoryId);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    public IActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Update(category);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Category edited successfully";
            return RedirectToAction("Index", "Category");
        }

        return View();
    }

    public IActionResult Delete(int? categoryId)
    {
        if (categoryId is null or 0)
        {
            return NotFound();
        }

        Category? category = _unitOfWork.Category.Get(c => c.Id == categoryId);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? categoryId)
    {
        Category? category = _unitOfWork.Category.Get(c => c.Id == categoryId);

        if (category == null)
        {
            return NotFound();
        }

        _unitOfWork.Category.Delete(category);
        _unitOfWork.SaveChanges();
        TempData["success"] = "Category deleted successfully";

        return RedirectToAction("Index", "Category");
    }
}
