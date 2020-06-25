using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company;
            
            if(id == null)
            {
                //create
                company = new Company();
                return View(company);
            }
            //edit
            company = _unitOfWork.Company.Get(id.GetValueOrDefault());
            if(company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {

            if (ModelState.IsValid)
            {
                if(company.Id == 0)
                {
                    //create
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    //edit
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if(company.Id != 0)
                {
                    company = _unitOfWork.Company.Get(company.Id);
                }
                return View(company);
            }

            Debug.WriteLine("ModelState not valid");
            return View(company);
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var objFromDb = _unitOfWork.Company.GetAll();
            return Json(new { data = objFromDb });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Company.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = "false", message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = "true", message = "Delete Successful" });
        }

        #endregion


    }
}