using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using WLSPL_CRM_2.Models;
using WLSPL_CRM_2.repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static WLSPL_CRM_2.Models.Quatation;

namespace WLSPL_CRM_2.Controllers
{
    public class QuotationController : Controller
    {
        private readonly IleadRepo _leadRepo;
        private readonly IQuotationRepo _quotationRepo;
        private readonly IServicesRepo _ServicesRepo;
       
        public QuotationController(IleadRepo leadRepo, IQuotationRepo quotationRepo, IServicesRepo servicesRepo)
        {
            _leadRepo = leadRepo;
            _quotationRepo = quotationRepo;
            _ServicesRepo = servicesRepo;
        }
        public async Task<IActionResult> Index()
        {

            if (TempData["LeadCode"] != null && !string.IsNullOrEmpty(TempData["LeadCode"].ToString()))
            {
                string[] values = TempData["LeadCode"].ToString().Split(',');

                ViewBag.Leadcode = values[0];
                ViewBag.CompnayName = values[1];
            }

            var DETAILSJson = TempData["QuotationDeatils"] as string;
            var ItemDetailsJson = TempData["QuotationItemDetailSS"] as string;

            if (!string.IsNullOrEmpty(DETAILSJson))
            {

                var Quatation = JsonConvert.DeserializeObject<Quatation>(DETAILSJson);
                var ITemdeatils = JsonConvert.DeserializeObject<Quatation>(ItemDetailsJson);
                ViewBag.company = Quatation;
                //ViewBag.Items = ITemdeatils;
                var Quatationlist = new List<Quatation> { Quatation };
                var ITEMS = new ITEM
                {
                    Items = ITemdeatils.Items
                };
                ViewBag.Items = new List<SelectListItem>
                {
                     new SelectListItem { Text = "-- Select Item --", Value = "0", Disabled = true },
                     new SelectListItem { Text = "GST@18", Value = "18", Disabled = true },
                      new SelectListItem { Text = "GST@5%", Value = "5", Disabled = true }
                };
                foreach (var item in ITEMS.Items)
                {
                    // Add dynamic items to ViewBag.Items
                    ViewBag.Items.Add(new SelectListItem
                    {
                        Text = item.Tax.ToString(),
                        Value = item.Tax.ToString()
                    });
                }

                var Services = await _ServicesRepo.GetDepartment(Action: "Getservices");
                var serviceslist = Services.Select(s => new Services { ServicesDesc = s.ServicesDesc });

                ViewBag.serviceslist = serviceslist;

                ViewBag.Items = ITEMS.Items;
                ViewBag.Tax = ViewBag.Items;
                return View(Quatationlist);

            }
            else
            {

                var department = await _ServicesRepo.GetDepartment("GetDepartment");
                var departmentList = department.Select(d => new Department
                {
                    ID = d.ID,
                    DepartmentName = d.DepartmentName,
                    ServicesDesc = d.ServicesDesc,
                    IsActive = d.IsActive,
                    CreatedOn = d.CreatedOn,
                    CreatedBy = d.CreatedBy
                }).ToList();
                var companies = await _leadRepo.Getcompanies(Action: "GetCompany");
                var Services = await _ServicesRepo.GetDepartment(Action : "Getservices");
                var serviceslist = Services.Select(s => new Services { ServicesDesc = s.ServicesDesc });
                var companyList = companies.Select(c => new LeadGenration
                {
                    CompanyName = c.CompanyName
                }).ToList();
                ViewBag.DepartmentName = departmentList;
                ViewBag.comapnylsit = companyList;
                ViewBag.serviceslist = serviceslist;
                return View(companyList);
            }

        }
        public async Task<IActionResult> CheckCompanyAndGst(string gstNo, string companyName)
        {

            string action = "GetcomapnyDetails";

            var Result = await _quotationRepo.checkcompaniesdetails(action, companyName);
            var Details = Result.FirstOrDefault(p => p.Companyname == companyName);

            if (Details == null)
            {
                return NotFound();
            }
            var CompanyDetails = new
            {
                Gstno = Details.Gstno,
                Billingaddress = Details.Billingaddress,
                Shippingaddress = Details.Shippingaddress,
                Emailid = Details.Emailid,
                Mobileno = Details.Mobileno

            };

            return Json(CompanyDetails);


        }

        [HttpPost]
        public async Task<IActionResult> SubmitDeatils([FromBody] Quatation Model)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(Model.Items);
                string userName = HttpContext.Session.GetString("userName");
                //Model.CreatedBy = userName;

                var result = await _quotationRepo.SubmitDetails(Model, Action: "InsertHdrdeatils");
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

        public async Task<ActionResult> GetQuotationlist(Quatation model)
        {
            string action = "GetListDetails";
            var Result = await _quotationRepo.GetQuotationlist(action, model);
            return View(Result);
        }

        [HttpGet]
        public async Task<ActionResult> GetcompanybyID(string ID)
        {

            var DETAILS = await _quotationRepo.GetQutationbyId(ID);

            var ItemDetails = await _quotationRepo.GetQutationDetailsbyId(ID);
            if (DETAILS == null)
            {

                return View("Error", new { message = "company not found" });
            }
            TempData["QuotationDeatils"] = JsonConvert.SerializeObject(DETAILS);
            TempData["QuotationItemDetailSS"] = JsonConvert.SerializeObject(ItemDetails);
            //ViewData["DETAILSS"] = JsonConvert.SerializeObject(DETAILS);
            //ViewData["ItemDetailSS"] = JsonConvert.SerializeObject(ItemDetails);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDeatils([FromBody] Quatation Model)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(Model.Items);
                string userName = HttpContext.Session.GetString("userName");
                //Model.CreatedBy = userName;

                var result = await _quotationRepo.SubmitDetails(Model, Action: "UpdateHeaderDetails");
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
        public async Task<IActionResult> DeleteQuoatation(string ID)
        {
            string userName = HttpContext.Session.GetString("userName");
            if (string.IsNullOrEmpty(userName))
            {

                TempData["Error"] = "User session expired. Please log in again.";
                return RedirectToAction("Login", "Account");
            }
            var result = await _quotationRepo.DeleteQuotation(ID, CreatedBy: userName);
            if (result != null)
            {
                TempData["Save_Record"] = "Record saved successfully!";
                return RedirectToAction("Index", "Alert");
            }
            return View();
        }
        public async Task<IActionResult> GetbyDepartment (string Department)
        {

            string action = "Getservicesbydepartemnt";
            var Result = await _quotationRepo.GetservicesDetails(action , Department);

            var companyDetails = new
            {
                Results = Result
               .Where(r => r.ServicesDesc != null)   
                .Distinct()                           
                .ToList()                             
            };

            return Json(companyDetails);
        }

        public async Task<IActionResult> Getpricebyservice(string service)
        {
            string action = "Getpricebyservices";
            var Result = await _quotationRepo.GetservicesDetails(action, service);
    
            var serviceDetails = new
            {
                Results = Result
               .Where(r => r.Price != null)
                .Distinct()
                .ToList()
            };

            return Json(serviceDetails);
        }
    }
}

