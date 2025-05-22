using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using WLSPL_CRM_2.Models;
using WLSPL_CRM_2.repository;

namespace WLSPL_CRM_2.Controllers
{
    public class LeadController : Controller
    {
        private readonly IleadRepo _LeadRepo;
        private readonly IcomapnymasterRepo _companymaster;
        private readonly IUserRegistrationRepo _UserRegistrationRepo;
        public LeadController(IleadRepo LeadRepo, IcomapnymasterRepo companymaster, IUserRegistrationRepo userRegistrationRepo)
        {
            _LeadRepo = LeadRepo;
            _companymaster = companymaster;
            _UserRegistrationRepo = userRegistrationRepo;
        }

        //lead List For users
        [HttpGet]
        public async Task<ActionResult> GetLeadList(LeadGenration Model)
        {
            var role = HttpContext.Session.GetString("Role");
            string UserID = HttpContext.Session.GetString("UserID");
            string action = "GetLeadrecord";

            var leads = await _LeadRepo.GetLeadlist(action, Model, role, UserID);          

            if (leads == null)
            {
                return View("Error");
            }

            var newLead = new LeadGenration();

            // Create the tuple expected by the view
            var modelTuple = (leads, newLead);

            return View(modelTuple);
        }


        public async Task<IActionResult> Getleadform()
        {


            try
            {
                var leadsJson = TempData["Leads"] as string;

                if (!string.IsNullOrEmpty(leadsJson))
                {

                    var leads = JsonConvert.DeserializeObject<LeadGenration>(leadsJson);
                    ViewBag.Leads = leads;
                    return View();
                }
                else
                {

                    string Action = "Getcode";
                    var leadGenrations = await _LeadRepo.GetLeadcode(Action);
                    var companies = await _LeadRepo.Getcompanies(Action: "GetCompany");
                    var leadCode = leadGenrations.FirstOrDefault()?.Leadcode ?? string.Empty;         
                    ViewBag.LeadCode = leadCode;                
                    return View();
                }


            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> SubmitDeatils(LeadGenration Model)
        {
            try
            {
                string userName = HttpContext.Session.GetString("UserID");
                Model.Createdby = userName;
                var result = await _LeadRepo.SubmitDetails(Model);
                if (result != null)
                {
                    TempData["Save_Record"] = "Entry saved Successfully..";
                    TempData["URL"] = "/Lead/Getleadlist";
                    TempData["icon"] = "success";
                    return RedirectToAction("Index", "Alert");

                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["Save_Record"] = "Please fill data..";
                TempData["URL"] = "/Lead/Getleadlist";
                TempData["icon"] = "error";
                return RedirectToAction("Index", "Alert");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRequest(LeadGenration model)
        {
            try
            {
              
                    string userName = HttpContext.Session.GetString("UserID");
                    if (string.IsNullOrEmpty(userName))
                    {
                        TempData["Save_Record"] = "User session expired. Please log in again.";
                        TempData["URL"] = "/Login/Index";
                        TempData["icon"] = "error";
                        return RedirectToAction("Index", "Alert");

                    }

                    model.Createdby = userName;
                    var result = await _LeadRepo.UpdateDetails(model);

                    if (result != null)
                    {

                        TempData["Save_Record"] = "Entry Updated Successfully..";
                        TempData["URL"] = "/Lead/Getleadlist";
                        TempData["icon"] = "success";
                        return RedirectToAction("Index", "Alert");
                    }

                


                return RedirectToAction("Getleadlist", "Lead");
            }
            catch (Exception ex)
            {
                TempData["Save_Record"] = "Please fill data..";
                TempData["URL"] = "/Lead/Getleadlist";
                TempData["icon"] = "error";
                return RedirectToAction("Index", "Alert");
            }
        }
        [HttpGet]
        public async Task<ActionResult> GetLeadbyID(string ID)
        {

            var leads = await _LeadRepo.GetleadbyId(ID);
            if (leads == null)
            {

                return View("Error", new { message = "Lead not found" });
            }
            //ViewBag.Leads = leads;
            TempData["Leads"] = JsonConvert.SerializeObject(leads);
            return RedirectToAction("Getleadform");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteLead(string ID)
        {
            string userName = HttpContext.Session.GetString("userName");
            if (string.IsNullOrEmpty(userName))
            {
                TempData["Save_Record"] = "User session expired. Please log in again.";
                TempData["URL"] = "/Login/Index";
                TempData["icon"] = "error";
                return RedirectToAction("Index", "Alert");
            }
            var result = await _LeadRepo.DeleteReord(ID, CreatedBy: userName);
            if (result != null)
            {
                TempData["Save_Record"] = "Record Deleted successfully!";
                TempData["URL"] = "/Lead/Getleadlist";
                TempData["icon"] = "success";
                return RedirectToAction("Index", "Alert");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Createconnection(LeadGenration Model)
        {
            try
            {
                string userName = HttpContext.Session.GetString("UserID");
                Model.Createdby = userName;
                Model.CreatedOn = DateTime.Now;
                if (!string.IsNullOrEmpty(Model.UserName))
                {
                    var result = await _LeadRepo.submitconnetciondetails(Model);
                    if (result != null)
                    {
                        TempData["Save_Record"] = "Approved Successfully..";
                        TempData["URL"] = "/Lead/Getleadlist";
                        TempData["icon"] = "success";
                        return RedirectToAction("Index", "Alert");
                    }
                  
                }
                else
                {
                    TempData["Save_Record"] = "Please try again.";
                    TempData["URL"] = "/Lead/Getleadlist";
                    TempData["icon"] = "error";
                    return RedirectToAction("Index", "Alert");
                }
                return RedirectToAction("Getleadlist", "Lead");

            }
            catch (Exception ex)
            {

                TempData["Save_Record"] = "Please try again.";
                TempData["URL"] = "/Lead/Getleadlist";
                TempData["icon"] = "error";
                return RedirectToAction("Index", "Alert");
            }
        }

        [HttpPost]
        public async Task<JsonResult> SearchCompany(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(new List<object>());
            }

            // Await the asynchronous call properly
            var companies = await _companymaster.SearchCompanyAsync(name);

            // Filter and shape the result
            var result = companies
                .Select(c => new
                {
                    value = c.Id,
                    text = c.CompanyName
                })
                .ToList();

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> SearchExecutive(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(new List<object>());
            }

            // Await the asynchronous call properly
            var companies = await _UserRegistrationRepo.SearchExecutiveAsync(name);

            // Filter and shape the result
            var result = companies
                .Select(c => new
                {
                    value = c.ID,
                    text = c.UserName
                })
                .ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> SendForQuot(string Leadcode)
        {
            TempData["LeadCode"] = Leadcode;

            return RedirectToAction("Index", "Quotation");
        }
    }
}
