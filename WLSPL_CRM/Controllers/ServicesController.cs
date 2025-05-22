using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WLSPL_CRM_2.Models;
using WLSPL_CRM_2.repository;

namespace WLSPL_CRM_2.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IServicesRepo _ServicesRepo;
        public ServicesController(IServicesRepo servicesRepo)
        {
            _ServicesRepo = servicesRepo;
        }
        public async Task<IActionResult> Createservices()
        {
            var department = await _ServicesRepo.GetDepartment("GetDepartment");
            var departmentList = department.Select(d => new Department
            {
                ID = d.ID,
                DepartmentName = d.DepartmentName,
                IsActive = d.IsActive,
                CreatedOn = d.CreatedOn,
                CreatedBy = d.CreatedBy
            }).ToList();
            ViewBag.Department = departmentList;
            return View(departmentList);
        }
        [HttpPost]

        public async Task<IActionResult> SubmitDeatils(Services Model)
        {
            try
            {
                string userName = HttpContext.Session.GetString("userName");
                Model.CreatedBy = userName;
                var result = await _ServicesRepo.Submitservices(Model, Action: "Insertservices");
                if (result != null)
                {

                    TempData["Save_Record"] = "Save_Record";
                    return RedirectToAction("Index", "Alert");

                }

                return View();
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Getlist(Services model)
        {
            string action = "GetservicesDetails";
            var Result = await _ServicesRepo.GetDepartment(action);
            if (Result != null)
            {
                List<string> departments = Result.Where(r => r.Department != null).Select(r => r.Department).ToList();
                ViewBag.Departments = departments;
            }

            return View(Result);
        }
        [HttpGet]
        public async Task<ActionResult> GetlistForAdmin(Services model)
        {
            string action = "GetservicesDetails";
            var Result = await _ServicesRepo.GetDepartment(action);
            if (Result != null)
            {
                List<string> departments = Result.Where(r => r.Department != null).Select(r => r.Department).ToList();
                ViewBag.Departments = departments;
            }

            return View(Result);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateDetails(Services Model)
        {
            try
            {
                string userName = HttpContext.Session.GetString("userName");
                Model.CreatedBy = userName;
                var result = await _ServicesRepo.Submitservices(Model, Action: "Updateservices");
                if (result != null)
                {

                    TempData["Save_Record"] = "Save_Record";
                    return RedirectToAction("Index", "Alert");

                }

                return View();
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }
        public async Task<IActionResult> DeleteService(string ID)
        {

            string action = "Deleteservices";
            var Result = await _ServicesRepo.Delete(ID, action);
            if (Result == 1)
            {
                return Json("Valid");
            }
            else
            {
                return Json("Non Valid");
            }
        }



    }
}


