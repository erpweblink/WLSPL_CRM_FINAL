using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WLSPL_CRM_2.Models;
using WLSPL_CRM_2.repository;

namespace WLSPL_CRM_2.Controllers
{
    public class ProformaController : Controller
    {
        private readonly IProformaInvoiceRepo _proformaRepo;
        public ProformaController(IProformaInvoiceRepo proformaRepo)
        {
            _proformaRepo = proformaRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _proformaRepo.GetProformalist("GetList");
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var ServiceList = await _proformaRepo.GetServiceList("GETServiceList");
            ViewBag.ProductList = ServiceList.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.ServicesDesc
            }).ToList();


            var ProformaCode = await _proformaRepo.GetProformaNumber("DB_CRMERPWLSPL.FN_GetProformaCode()");
            ProformaInvoiceHdr obj = new ProformaInvoiceHdr
            {
                ProformaNo = ProformaCode,
                ProformaDate = DateTime.Now.Date
            };
            var companies = await _proformaRepo.checkcompaniesdetails("GETCOMPANYRECORD");
            var companyList = companies.Select(c => new Company
            {
                CompanyName = c.CompanyName
            }).ToList();
            ViewBag.comapnylsit = companyList;
            ViewBag.QuotationNo = new List<SelectListItem>();
            return View(obj);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProformaInvoiceHdr model)
        {
            model.CreatedBy = HttpContext.Session.GetString("userName");

            var result = await _proformaRepo.SubmitDetails(model, "InsertHdrDetails");
            if (result != null)
            {
                TempData["Save_Record"] = "Record Save successfully!";
                TempData["URL"] = "/Proforma/Index";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Alert") });
            }

            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerData(string CompanyName)
        {
            string[] values = CompanyName.Split('/');

            if (values[1] != "Direct")
            {
                var quotationData = await _proformaRepo.checkquotationdetails("GETQuotationRECORD", values[0]);

                var quotationNo = quotationData.Select(ba => new SelectListItem
                {
                    Value = ba.Quotationno?.ToString(),
                    Text = ba.Quotationno?.ToString()
                }).ToList();

                var result = new
                {
                    QuotationData = quotationNo,
                };

                return Json(result);
            }
            else
            {
                var CustomerData = await _proformaRepo.GetCustdetails("GetCustdetails", values[0]);

                var result = new
                {
                    BillAddress = CustomerData.BillAddress,
                    Gstno = CustomerData.GSTNo,
                    QuotationData = "No quotatins"
                };

                return Json(result);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetQuotationNoData(string QuotationNo)
        {
            var quotationData = await _proformaRepo.Getquotationdetails("GETQuotationData", QuotationNo);
            //await Task.Delay(5000);
            var quotationdtlsData = await _proformaRepo.Getquotationdetails("GETQuotationDtlsData", quotationData.ID);

            List<ProformaInvoiceDtls> resList = new List<ProformaInvoiceDtls>();

            foreach (var item in quotationdtlsData)
            {
                var res = new ProformaInvoiceDtls
                {
                    ProductDescription = item.Item,
                    SACCode = item.Hsn,
                    Qty = item.Qty,
                    Rate = item.Price,
                    Total = item.Qty * item.Price,
                    Discount = item.Discount,
                    DiscountAmt = item.DiscountAmount,
                    Tax = item.Tax,
                    TaxAmt = item.TaxAmount,
                    GrandTotal = item.Amount
                };

                resList.Add(res);
            }
            var result = new
            {
                BillingAddress = quotationData.Billingaddress,
                GstNumber = quotationData.Gstno,
                QuotationDtls = resList
            };

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetProductDetails(string productId)
        {
            var ServiceDetails = await _proformaRepo.Getquotationdetails("GETServiceList", productId);

            return Json(ServiceDetails);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var result = await _proformaRepo.GetProformabyId("GetProformaByID", Id);

            var ServiceList = await _proformaRepo.GetServiceList("GETServiceList");
            ViewBag.ProductList = ServiceList.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.ServicesDesc
            }).ToList();
            ViewBag.ProductDetsils = await _proformaRepo.GetProformabyId("GetProformaDeailsByID", result.Id.ToString());

            return View(result);

        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] ProformaInvoiceHdr model)
        {

            model.CreatedBy = HttpContext.Session.GetString("userName");

            var result = await _proformaRepo.SubmitDetails(model, "UpdateHeaderDetails");
            if (result != null)
            {
                TempData["Save_Record"] = "Record Updated successfully!";
                TempData["URL"] = "/Proforma/Index";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Alert") });
            }

            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            var result = await _proformaRepo.SubmitDetails(new ProformaInvoiceHdr{ Id = Convert.ToInt32(Id) }, "DeleteProforma");
            if (result != null)
            {
                TempData["Save_Record"] = "Record Deleted successfully!";
                TempData["URL"] = "/Proforma/Index";
                return RedirectToAction("Index", "Alert");
            }
            return View("Index");
        }

    }
}
