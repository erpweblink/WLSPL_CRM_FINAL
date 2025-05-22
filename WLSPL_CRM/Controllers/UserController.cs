using Microsoft.AspNetCore.Mvc;
using WLSPL_CRM_2.Models;
using WLSPL_CRM_2.repository;


namespace WLSPL_CRM_2.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRegistrationRepo _UserRepo;
        public UserController(IUserRegistrationRepo userRegistarion)
        {
            _UserRepo = userRegistarion;
        }
        public IActionResult GetResterForm()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> UserRegister(RegisterUser Model) 
        {

            try
            {
                var result = await _UserRepo.RegisterUser(Model);

                if (result != null)
                {

                    TempData["Save_Record"] = "You Registration Completed Successfully...";
                    TempData["URL"] = "/Login/Index";
                    TempData["icon"] = "success";
                    TempData["Time"] = "2000";
                    return RedirectToAction("IndexLogin", "Alert");
                }
                else
                {
                    TempData["ErrorMessage"] = "Email or password is incorrect.";
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to login: {ex.Message}");
                return View(Model.UserName, Model.Password);
            }

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            string userId = Convert.ToString(id);
            var user = await _UserRepo.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);

        }
        [HttpPost]
        public async Task<IActionResult> Updateuser(RegisterUser User)
        {
            int res = await _UserRepo.UpdateUserDetails(User);
            if (res != 0)
            {
                TempData["Save_Record"] = "User Updated Successfully";
                TempData["URL"] = "/Dashboard/Index";
                TempData["icon"] = "success";
                TempData["Time"] = "2000";
                return RedirectToAction("IndexLogin", "Alert");
            }
            else
            {
                return View();
            }
        }

    }

}

