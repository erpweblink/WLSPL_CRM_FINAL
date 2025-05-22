using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using WLSPL_CRM_2.Models;
using WLSPL_CRM_2.repository;
using static WLSPL_CRM_2.Models.Quatation;

namespace WLSPL_CRM_2.Controllers
{
    public class WorkorderController : Controller
    {
        private readonly IWorkorderRepo _WorkorderRepo;
        public WorkorderController(IWorkorderRepo workorderRepo)
        {
            _WorkorderRepo = workorderRepo;
        }

        public async Task<IActionResult> CreateWorkorder(string Quotationno)
        {

            var DETAILSJson = TempData["WoDetails"] as string;
            var ItemDetailsJson = TempData["WoItemdetails"] as string;
            var success = HttpContext.Session.GetString("Save_Record");


            if (success == null)
            {
                if (!string.IsNullOrEmpty(DETAILSJson))
            {

               






                    var quotationDetails = JsonConvert.DeserializeObject<Workorder>(DETAILSJson);
                    var Deatils = JsonConvert.DeserializeObject<Workorder>(ItemDetailsJson);
                    List<Workorder> model = new List<Workorder>();

                    if (quotationDetails != null)
                    {
                        model.Add(new Workorder
                        {
                            QuotationNo = quotationDetails.QuotationNo,
                            CustomerName = quotationDetails.CustomerName,
                            GSTNO = quotationDetails.GSTNO,
                            WONO = quotationDetails.WONO,
                            WOcreateDate = quotationDetails.WOcreateDate,
                            BalanceAmount = quotationDetails.BalanceAmount,
                            TotalFullamount = quotationDetails.TotalFullamount,
                            ID = quotationDetails.ID,
                        });
                    }

                    ViewBag.Items = Deatils?.Items;
                    ViewBag.WorkorderList = model;

                    return View();
                }
                else
                {
                    string decodedID = WebUtility.UrlDecode(Quotationno);
                    var quotationDetails = await _WorkorderRepo.GetQutationbyQuotaion(decodedID);
                    var Deatils = await _WorkorderRepo.GetQutationDetails(decodedID);
                    var No = await _WorkorderRepo.GetWorkorderNo(Action: "WONO");

                    List<Workorder> model = new List<Workorder>();

                    if (quotationDetails != null)
                    {
                        model.Add(new Workorder
                        {
                            QuotationNo = quotationDetails.QuotationNo,
                            Companyname = quotationDetails.Companyname,
                            GSTNO = quotationDetails.GSTNO,
                            BalanceAmount = quotationDetails.BalanceAmount,
                            TotalFullamount = quotationDetails.TotalFullamount
                        });
                    }

                    ViewBag.Items = Deatils?.Items;
                    ViewBag.WorkorderList = model;
                    ViewBag.WoNo = No;

                    return View();
                }

            }

            else
            {
                HttpContext.Session.Remove("Save_Record");

                return RedirectToAction("Getworkorderlist", "Workorder");
            }



        }




            [HttpPost]
            public async Task<IActionResult> SubmitDeatils([FromBody] Workorder Model)
            {
                try
                {
                    var jsonString = JsonConvert.SerializeObject(Model.Items);
                    string userName = HttpContext.Session.GetString("userName");
                    var result = await _WorkorderRepo.SubmitDetails(Model, Action: "InsertHdrdeatils");
                    if (result != null)
                    {
                        TempData["Save_Record"] = "Submited Successfully";
                        TempData["URL"] = "/Workorder/Getworkorderlist";
                        TempData["icon"] = "success";
                        return RedirectToAction("IndexLogin", "Alert");

                    }

                    return View();
                }
                catch (Exception ex)
                {

                    return View("Error");
                }
            }
            [HttpGet]
            public async Task<ActionResult> Getworkorderlist(Workorder model)
            {
                string action = "GetWoList";
                var Result = await _WorkorderRepo.GetWorkorderList(action, model);
                return View(Result);
            }

            [HttpGet]
            public async Task<ActionResult> GetWorkorderbyID(string ID)
            {
                var WoDetails = await _WorkorderRepo.GetbyWorkdetailsbyId(ID);
                var WoItemdetails = await _WorkorderRepo.GetWorkItemDetailsbyId(ID);
                if (WoDetails == null)
                {

                    return View("Error", new { message = "company not found" });
                }
                TempData["WoDetails"] = JsonConvert.SerializeObject(WoDetails);
                TempData["WoItemdetails"] = JsonConvert.SerializeObject(WoItemdetails);
                return RedirectToAction("CreateWorkorder");
            }

            [HttpPost]
            public async Task<IActionResult> UpdateDeatils([FromBody] Workorder Model)
            {
                try
                {
                    var jsonString = JsonConvert.SerializeObject(Model.Items);
                    string userName = HttpContext.Session.GetString("userName");
                    var result = await _WorkorderRepo.SubmitDetails(Model, Action: "UpdateHdrdetails");
                    if (result != null)
                    {
                    HttpContext.Session.SetString("WoDetails", JsonConvert.SerializeObject(Model));
                    HttpContext.Session.SetString("Save_Record", "Updated Successfully");
                    TempData["Save_Record"] = "Updated Successfully";
                    return Json(new { redirectUrl = Url.Action("CreateWorkorder", "Workorder") });
                }
                    else
                    {
                        return Json(new { redirectUrl = Url.Action("Getworkorderlist", "Workorder") });
                    }






                }
                catch (Exception ex)
                {

                    return View("Error");
                }
            }


        public async Task<IActionResult> Deleteworkorder(string ID)
        {
            try
            {
                string userName = HttpContext.Session.GetString("userName");
                if (string.IsNullOrEmpty(userName))
                {

                    TempData["Error"] = "User session expired. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }
                var result = await _WorkorderRepo.DeleteWorkorder(ID, CreatedBy: userName);
                if (result != null)
                {
                    TempData["Save_Record"] = "Deleted Successfully";
                    TempData["URL"] = "/Workorder/Getworkorderlist";
                    TempData["icon"] = "success";
                    return RedirectToAction("IndexLogin", "Alert");

                }
                return View();
            }
            catch (Exception ex)
            {

                throw;
            }
        }


    }
}
