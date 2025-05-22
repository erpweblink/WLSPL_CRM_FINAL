using Microsoft.AspNetCore.Mvc;
using WLSPL_CRM_2.Models;
using WLSPL_CRM_2.repository;

namespace WLSPL_CRM_2.Controllers
{
    public class DealController : Controller
    {
        private readonly IDealRepo _dealRepo;

        public DealController(IDealRepo dealRepo)
        {
            _dealRepo = dealRepo;
        }

        // Fetch and display deals
        [HttpGet]
        public async Task<IActionResult> Index()
        
        {
            var deals = await _dealRepo.GetAllDeals();
            return View(deals);
        }

        // Submit deal details
        [HttpPost]
        public async Task<IActionResult> SubmitDetails([FromBody] Deal model)
        {
            if (model == null || string.IsNullOrEmpty(model.Company))
            {
                return Json(new { success = false, message = "Company name is required." });
            }

            try
            {
                string userName = HttpContext.Session.GetString("userName");
                model.CreatedBy = userName;

                int result = await _dealRepo.SubmitDetails(model, "Insert");
                return Json(new { success = result > 0, message = result > 0 ? "Deal saved successfully!" : "Failed to save deal." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


    }
}
