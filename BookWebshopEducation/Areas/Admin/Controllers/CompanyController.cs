using BookWebshopEducation.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using BookWebshopEducation.Models.Models;
using BookWebshopEducation.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using BookWebshopEducation.Utility;
using Microsoft.AspNetCore.Authorization;
namespace BookWebshopEducation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Role_Admin)]
    public class CompanyController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();
            return View(companies);
        }

        public IActionResult Upsert(int? id)
        {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();

            if (id == null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                return View(companies.FirstOrDefault(c => c.Id == id));
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                }

                _unitOfWork.SaveChanges();
                //TempData["success"] = "Product created successfully";
                return RedirectToAction("Index", "Company");
            }

            return View(company);
        }

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> company = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = company });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var company = _unitOfWork.Company.Get(p => p.Id == id);

            if (company == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }

            _unitOfWork.Company.Delete(company);
            _unitOfWork.SaveChanges();

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
