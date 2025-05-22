using Microsoft.AspNetCore.Mvc;
using WLSPL_CRM_2.repository;

namespace WLSPL_CRM_2.Controllers
{
    public class prospectController(Iprospectrepo prospectRepo) : Controller
    {
        private readonly  Iprospectrepo _prospectRepo = prospectRepo;

        public async Task<IActionResult> Getprospectlist()
        {
            string action = "Getprospectdetails";
            var Prospect = await _prospectRepo.GetProspectslsit(action);
            var pendingcount = await _prospectRepo.pendingcount();
            var Discussioncount = await _prospectRepo.Discussioncount();
            var Finalizecount = await _prospectRepo.Finalizecount();
            var Cancelpartycount = await _prospectRepo.CancelPartycount();
            if (Prospect == null)
            {
                return View("Error");
            }

            ViewBag.Pendingcount = pendingcount;
            ViewBag.Discussioncount = Discussioncount;  
            ViewBag.Finalizecount = Finalizecount;  
            ViewBag.Cancelpartycount = Cancelpartycount;    

            return View(Prospect);
        }

    }
}
