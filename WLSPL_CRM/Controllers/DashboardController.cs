using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WLSPL_CRM.Models;
using WLSPL_CRM_2.repository;

namespace WLSPL_CRM_2.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUserRegistrationRepo _Repo;
        public DashboardController(IUserRegistrationRepo Repo)
        {
            _Repo = Repo;
        }

        public async Task<IActionResult> Index()
        {
            var users = (await _Repo.UserListOrg("GetOrgData"))
                .Select(u => new UserViewModel
                {
                    ID = u.ID,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    EmailId = u.EmailId,
                    MobileNo = u.MobileNo,
                    Password = u.Password,
                    ConfirmPassword = u.ConfirmPassword,
                    Role = u.Role,
                    Department = u.Department,
                    RelationID = u.RelationID,
                    Children = new List<UserViewModel>() // important for hierarchy
                }).ToList();

            // Create a lookup dictionary
            var userDict = users.ToDictionary(u => u.ID);

            // Build hierarchy by mapping parent-child relationships
            foreach (var user in users)
            {
                if (user.RelationID.HasValue && userDict.ContainsKey(user.RelationID.Value))
                {
                    userDict[user.RelationID.Value].Children.Add(user);
                }
            }

            // Identify the root user (e.g., Admin)
            var topUser = users.FirstOrDefault(u => u.Role == "Admin");

            // Return view with hierarchy
            return View("OrgChart", topUser);
        }

        public async Task<IActionResult> Showprofile()
        {
            string userId = HttpContext.Session.GetString("UserID");
            var obj = await _Repo.GetUserById(userId);
            if (string.IsNullOrEmpty(obj.PhotoPath))
            {
                obj.PhotoPath = "";
            }
            return View(obj);
        }



    }
}