using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Reflection.Emit;
using WLSPL_CRM_2.Models;
using WLSPL_CRM_2.repository;

namespace WLSPL_CRM_2.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IcomapnymasterRepo _companymaster;
        public CompanyController(IcomapnymasterRepo CompanymasterRepo)
        {
            _companymaster = CompanymasterRepo;
        }
        public async Task<IActionResult> Getcomapnyform()
        {
            var companysJson = TempData["company"] as string;

            if (!string.IsNullOrEmpty(companysJson))
            {

                var company = JsonConvert.DeserializeObject<Company>(companysJson);
                ViewBag.company = company;
                var companyList = new List<Company> { company };


                ViewBag.SupplyTypes = new List<SelectListItem>
            {
            new SelectListItem { Text = "B2B", Value = "B2B" },
            new SelectListItem { Text = "B2C", Value = "B2C" },
            new SelectListItem { Text = "SEZWOP", Value = "SEZWOP" },
            new SelectListItem { Text = "EXPWOP", Value = "EXPWOP" }
              };


                ViewBag.RegisterForList = new List<SelectListItem>
    {
        new SelectListItem { Text = "WLS", Value = "WLS" },
        new SelectListItem { Text = "WLSPL", Value = "WLSPL" }
    };







                ViewBag.SelectedSupplyType = company.supplytype;

                return View(companyList);
            }
            else
            {
                string action = "GetCompanycode";
                var compcode = await _companymaster.Getcompcode(action);
                var code = compcode.FirstOrDefault()?.CompanyCode ?? string.Empty;
                ViewBag.code = code;
                return View();

            }



        }
        [HttpPost]
        public async Task<IActionResult> SubmitDeatils(Company Model)
        {
            try
            {
                string userName = HttpContext.Session.GetString("userName");
                Model.CreatedBy = userName;
                var result = await _companymaster.SubmitDetails(Model, Action: "Insert");
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
        public async Task<IActionResult> CheckCompanyAndGst(string gstNo, string companyName)
        {

            string action = "Getcomapny";
            var Result = await _companymaster.checkcomapnies(action, companyName, gstNo);
            var comapny = Result.FirstOrDefault()?.CompanyName ?? string.Empty;
            if (comapny == "")
            {
                return Json("Valid");
            }
            else
            {
                return Json("Non Valid");
            }
        }
        public async Task<ActionResult> Getlist(Company model)
        {
            string action = "GETCOMPANYRECORD";
            var companies = await _companymaster.GetLeadlist(action, model);
            return View(companies);
        }

        [HttpGet]
        public async Task<ActionResult> GetcompanybyID(string ID)
        {

            var company = await _companymaster.GetcompanybyId(ID);
            if (company == null)
            {

                return View("Error", new { message = "company not found" });
            }
            TempData["company"] = JsonConvert.SerializeObject(company);
            return RedirectToAction("Getcomapnyform");
        }
        [HttpGet]
        public async Task<IActionResult> Deletecompany(string ID)
        {
            string userName = HttpContext.Session.GetString("userName");
            if (string.IsNullOrEmpty(userName))
            {

                TempData["Error"] = "User session expired. Please log in again.";
                return RedirectToAction("Login", "Account");
            }
            var result = await _companymaster.DeleteReord(ID, CreatedBy: userName);
            if (result != null)
            {
                TempData["Save_Record"] = "Record saved successfully!";
                return RedirectToAction("Index", "Alert");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateDeatils(Company Model)
        {
            try
            {
                string userName = HttpContext.Session.GetString("userName");
                Model.CreatedBy = userName;
                var result = await _companymaster.SubmitDetails(Model, Action: "Update");
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

    }
}
