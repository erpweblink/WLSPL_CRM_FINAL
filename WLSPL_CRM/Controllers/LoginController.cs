using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WLSPL_CRM_2.Models;
using WLSPL_CRM_2.repository;

namespace WLSPL_CRM_2.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserRegistrationRepo _UserRepo;
        private object httpContext;

        public LoginController(IUserRegistrationRepo userRegistarion)
        {
            _UserRepo = userRegistarion;
        }
        public IActionResult Index()
        {
            TempData.Remove("UserMail");
            TempData.Remove("UserId");
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Getlogindetails(string userName, string password)
        {
            try
            {
                var result = await _UserRepo.LoginUser(userName, password);
                if (result != null)
                {
                    if (result.IsApproved != null && result.IsActive != null)
                    {

                        var Role = result?.Role ?? "NotApproved";
                        var ID = result.ID;
                        string IsApproved = result?.IsApproved?.ToString() ?? false.ToString();

                        var Department = result?.Department ?? "NotApproved";
                        var indiaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        string greeting = indiaTime.Hour switch
                        {
                            >= 5 and < 12 => "Good Morning",
                            >= 12 and < 17 => "Good Afternoon",
                            >= 17 and < 21 => "Good Evening",
                            _ => "Good Night"
                        };

                        string encryptedUserName = EncryptionHelper.Encrypt(userName);
                        string encryptedPassword = EncryptionHelper.Encrypt(password);

                        string URole = Role;
                        string image = result?.PhotoPath ?? "/assets/images/users/avatar-1.jpg".ToString();
                        string FullName = string.IsNullOrWhiteSpace((result.FirstName ?? "") + (result.LastName ?? "")) ? "No Name" : $"{result.FirstName ?? ""} {result.LastName ?? ""}".Trim();
                        string UserID = Convert.ToString(ID);
                        HttpContext.Session.SetString("Role", URole);
                        HttpContext.Session.SetString("UserID", UserID);



                        string Dep = Department;
                        HttpContext.Session.SetString("Department", Dep);
                        HttpContext.Session.SetString("userName", userName);
                        HttpContext.Session.SetString("FullName", FullName);
                        HttpContext.Session.SetString("greeting", greeting);
                        HttpContext.Session.SetString("Approved", IsApproved);
                        HttpContext.Session.SetString("Profile", image);

                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, userName),
                                new Claim("Role", result ?.Role ?? "NotApproved")
                            };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties
                        );
                    }
                    else
                    {
                        TempData["Save_Record"] = "User is not activated yet....";
                        TempData["URL"] = "/Login/Index";
                        TempData["icon"] = "error";
                        TempData["Time"] = "4000";
                        return RedirectToAction("IndexLogin", "Alert");
                    }
                    TempData["Save_Record"] = "Welcome " + userName + ".";
                    TempData["URL"] = "/Dashboard/Index";
                    TempData["icon"] = "success";
                    TempData["Time"] = "2000";
                    return RedirectToAction("IndexLogin", "Alert");
                }
                else
                {
                    TempData["Save_Record"] = "Email or password is incorrect.";
                    TempData["URL"] = "/Login/Index";
                    TempData["icon"] = "error";
                    TempData["Time"] = "3000";
                    return RedirectToAction("IndexLogin", "Alert");

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to login: {ex.Message}");
                return View(userName, password);
            }
        }
        [Authorize]
        public async Task<IActionResult> Getdashboard()
        {
            string action = "GetRequest";
            var USer = await _UserRepo.GetpendingRequest(action);
            int PendingCount = await _UserRepo.NotapprovedRecords("GetCount");

            var pendingInfo = new RegisterUser { pendingCount = PendingCount };
            var tupleModel = (USer, pendingInfo);

            ViewBag.SubAdminList = new List<SelectListItem>();
            ViewBag.Manager = new List<SelectListItem>();
            ViewBag.TeamLead = new List<SelectListItem>();

            return View(tupleModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDeatails(string userRole, string userdepartment, int ID, string IsApproved, string IsActivate, string subAdmin, string Manager, string teamLead)
        {
            try
            {
                bool IsActivated = false, IsApprove = false;
                if (IsApproved == "on")
                {
                    IsApprove = true;
                }
                if (IsActivate == "on")
                {
                    IsActivated = true;
                }
                var result = 0;
                if (subAdmin != null && Manager == null && teamLead == null)
                {
                    result = await _UserRepo.UpdateDeatils(userRole, userdepartment, ID, IsApprove, IsActivated, subAdmin);
                }
                if (Manager != null && teamLead == null)
                {
                    result = await _UserRepo.UpdateDeatils(userRole, userdepartment, ID, IsApprove, IsActivated, Manager);
                }
                if (Manager != null && teamLead != null)
                {
                    result = await _UserRepo.UpdateDeatils(userRole, userdepartment, ID, IsApprove, IsActivated, teamLead);
                }
                if (subAdmin == null && Manager == null && teamLead == null)
                {
                    result = await _UserRepo.UpdateDeatils(userRole, userdepartment, ID, IsApprove, IsActivated, null);
                }
                if (result > 0)
                {
                    TempData["Save_Record"] = "User Updated Successfully.";
                    TempData["URL"] = "/Login/GetDashboard";
                    TempData["icon"] = "success";
                    TempData["Time"] = "2000";
                    return RedirectToAction("IndexLogin", "Alert");
                }
                else
                {
                    TempData["Save_Record"] = "Something went wrong..";
                    TempData["URL"] = "/Login/GetDashboard";
                    TempData["icon"] = "error";
                    TempData["Time"] = "2000";
                    return RedirectToAction("IndexLogin", "Alert");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to login: {ex.Message}");
                return View(userRole, userdepartment);
            }
        }

        // 32-byte key (for AES-256)
        // You should use a secure key
        public class EncryptionHelper
        {
            private static string EncryptionKey = "6C9E8D2E2F8A6B4C9A5B3D93D12E9F9A"; // You should use a secure key

            // Encrypt the text
            public static string Encrypt(string plainText)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                    aesAlg.IV = new byte[16]; // AES requires a 16-byte IV, we can use a zero-filled array

                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                            {
                                using (StreamWriter sw = new StreamWriter(cs))
                                {
                                    sw.Write(plainText);
                                }
                            }
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }

            // Decrypt the text
            public static string Decrypt(string cipherText)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                    aesAlg.IV = new byte[16];

                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                        {
                            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader sr = new StreamReader(cs))
                                {
                                    return sr.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return View("Index");
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            var registerUser = new RegisterUser();
            var forgetPassword = new ForgetPasswordMaster();
            if (TempData.ContainsKey("UserMail") && !string.IsNullOrEmpty(TempData.Peek("UserMail")?.ToString()))
            {
                forgetPassword.userMail = TempData.Peek("UserMail")?.ToString();
                forgetPassword.userId = Convert.ToInt32(TempData.Peek("UserId")?.ToString() ?? "0");
                forgetPassword.NewPassword = string.Empty;
                forgetPassword.ConfirmPassword = string.Empty;
            }
            return View((registerUser, forgetPassword));
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(RegisterUser reg)
        {
            var result = await _UserRepo.ForgetUser(reg.UserName, reg.EmailId);
            if (result != null)
            {
                int val = GenerateSixDigitNumber();

                ForgetPasswordMaster frogs = new ForgetPasswordMaster
                {
                    userId = result.ID,
                    userMail = result.Email_ID,
                    Tempotp = val.ToString()
                };
                var OldValDelete = await _UserRepo.DeleteForgetData(result.Email_ID);
                if (OldValDelete == 1)
                {
                    var res = await _UserRepo.InsertForgetData(frogs);
                    if (res == 1)
                    {
                        string emailBody = $"<!DOCTYPE html> <html> <head> <meta charset=\"UTF-8\"> <title>OTP for Password Reset</title> <style> body {{ font-family: Arial, sans-serif; background-color: #f4f6f9; color: #333; padding: 20px; }} .container {{ background-color: #ffffff; padding: 25px; border-radius: 6px; box-shadow: 0px 0px 8px rgba(0,0,0,0.1); max-width: 600px; margin: auto; }} .header {{ text-align: center; padding-bottom: 20px; color: #007bff; }} .otp {{ font-size: 24px; font-weight: bold; color: #dc3545; margin: 20px 0; text-align: center; }} .footer {{ font-size: 12px; color: #888; margin-top: 30px; text-align: center; }} </style> </head> <body> <div class=\"container\"> <h2 class=\"header\">Password Reset Request</h2> <p>Hello,</p> <p>You recently requested to reset your password. Please use the OTP below to proceed:</p> <div class=\"otp\">{val}</div> <p>This OTP is valid for the next 10 minutes. If you did not request a password reset, you can safely ignore this email.</p> <p>Thanks,<br><div class=\"brand\">\r\n\t\t\t<a href=\"index.html\" class=\"logo\">\r\n\t\t\t\t<span>\r\n\t\t\t\t\t<img src=\"https://www.weblinkservices.net/assets-web/logo-main.png\" alt=\"logo-small\" class=\"logo-sm\">\r\n\t\t\t\t</span>\r\n\r\n\t\t\t</a>\r\n\t\t</div></p> <div class=\"footer\"> This is an automated message. Please do not reply to this email. </div> </div> </body> </html>";

                        await SendMailAsync(result.Email_ID, "Your OTP Code", emailBody);

                        TempData["UserMail"] = result.Email_ID;
                        TempData["UserId"] = result.ID;
                        TempData["OTP"] = val;
                        TempData["Save_Record"] = "OTP Send Successfully..";
                        TempData["URL"] = "/Login/ForgetPassword";
                        TempData["icon"] = "success";
                        TempData["Time"] = "2000";
                        return RedirectToAction("IndexLogin", "Alert");
                    }
                }
            }

            ModelState.AddModelError("", "Invalid username or email");
            TempData["Save_Record"] = "Invalid username or email..";
            TempData["URL"] = "/Login/ForgetPassword";
            TempData["icon"] = "error";
            TempData["Time"] = "2000";
            return RedirectToAction("IndexLogin", "Alert");
        }

        public async Task<bool> SendMailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var fromAddress = new MailAddress("testing@weblinkservices.net", "WebLink Service Pvt.Ltd");
                var toAddress = new MailAddress(toEmail);
                const string fromPassword = "Weblink@Testing#123";

                var smtp = new SmtpClient
                {
                    Host = "smtpout.secureserver.net", // e.g., smtp.gmail.com
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    await smtp.SendMailAsync(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log error (ex)
                return false;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(ForgetPasswordMaster reg)
        {
            if (reg.NewPassword == reg.ConfirmPassword)
            {

                var chkOTP = await _UserRepo.GetForgetPasswordRequest(reg.userId.ToString() ?? "0", reg.userMail ?? "0");
                if (chkOTP != null)
                {
                    if (chkOTP.Tempotp == reg.Tempotp)
                    {
                        var res = await _UserRepo.UpdateNewpassword(reg.NewPassword, reg.userId.ToString(), reg.userMail, reg.ConfirmPassword);
                        if (res == 1)
                        {
                            TempData["Save_Record"] = "Password SetSuccessfully..";
                            TempData["URL"] = "/Login/Index";
                            TempData["icon"] = "success";
                            TempData["Time"] = "2000";
                            return RedirectToAction("IndexLogin", "Alert");
                        }
                    }
                    else
                    {
                        TempData["Save_Record"] = "Please Check The OTP";
                        TempData["URL"] = "/Login/ForgetPassword";
                        TempData["icon"] = "error";
                        TempData["Time"] = "2000";
                        return RedirectToAction("IndexLogin", "Alert");
                    }
                }
            }
            TempData["Save_Record"] = "Please Check The Password";
            TempData["URL"] = "/Login/ForgetPassword";
            TempData["icon"] = "error";
            TempData["Time"] = "2000";
            return RedirectToAction("IndexLogin", "Alert");
        }
        public int GenerateSixDigitNumber()
        {
            Random random = new Random();
            return random.Next(100000, 1000000); // 100000 to 999999 inclusive
        }

        [HttpGet]
        public async Task<IActionResult> ShowList(string count)
        {
            var USer = await _UserRepo.GetpendingRequest("GetNotActiveList");

            ViewBag.SubAdminList = new List<SelectListItem>();
            ViewBag.Manager = new List<SelectListItem>();
            ViewBag.TeamLead = new List<SelectListItem>();

            return View(USer);
        }
        [HttpPost]
        public async Task<IActionResult> ApproveDeatails(string userRole, string userdepartment, int ID, string IsApproved, string IsActivate, string subAdmin, string Manager, string teamLead)
        {
            bool IsActivated = false, IsApprove = false;
            if (IsApproved == "on")
            {
                IsApprove = true;
            }
            if (IsActivate == "on")
            {
                IsActivated = true;
            }
            var result = 0;
            if (subAdmin != null && Manager == null && teamLead == null)
            {
                result = await _UserRepo.UpdateDeatils(userRole, userdepartment, ID, IsApprove, IsActivated, subAdmin);
            }
            if (Manager != null && teamLead == null)
            {
                result = await _UserRepo.UpdateDeatils(userRole, userdepartment, ID, IsApprove, IsActivated, Manager);
            }
            if (Manager != null && teamLead != null)
            {
                result = await _UserRepo.UpdateDeatils(userRole, userdepartment, ID, IsApprove, IsActivated, teamLead);
            }
            if (subAdmin == null && Manager == null && teamLead == null)
            {
                result = await _UserRepo.UpdateDeatils(userRole, userdepartment, ID, IsApprove, IsActivated, null);
            }
            if (result > 0)
            {
                var obj = await _UserRepo.GetUserById(ID.ToString());   
                if (obj.IsApproved == true && obj.IsActive == true)
                {
                    string emailBody = $"<!DOCTYPE html><html><head><meta charset=\"UTF-8\"><title>Profile Activated</title><style>body{{font-family:Arial,sans-serif;background-color:#f4f6f9;color:#333;padding:20px;}}.container{{background-color:#ffffff;padding:25px;border-radius:6px;box-shadow:0px 0px 8px rgba(0,0,0,0.1);max-width:600px;margin:auto;}}.header{{text-align:center;padding-bottom:20px;color:#28a745;}}.message{{font-size:18px;margin:20px 0;text-align:center;}}.footer{{font-size:12px;color:#888;margin-top:30px;text-align:center;}}</style></head><body><div class=\"container\"><h2 class=\"header\">Profile Activated Successfully</h2><p class=\"message\">Hello {obj.FirstName + " " + obj.LastName},</p><p class=\"message\">Your profile has been successfully activated. You can now log in and start using our services.</p><p class=\"message\">If you have any questions, feel free to contact our support team (Mobile No.- +91 93565 49427).</p><p>Thanks,<br><div class=\"brand\"><a href=\"https://www.weblinkservices.net\" class=\"logo\"><span><img src=\"https://www.weblinkservices.net/assets-web/logo-main.png\" alt=\"logo-small\" class=\"logo-sm\"></span></a></div></p><div class=\"footer\">This is an automated message. Please do not reply to this email.</div></div></body></html>";
                    await SendMailAsync(obj.EmailId, "Your Profile is Activated", emailBody);
                }

                TempData["Save_Record"] = "User Updated Successfully.";
                TempData["URL"] = "/Login/ShowList";
                TempData["icon"] = "success";
                TempData["Time"] = "2000";
                return RedirectToAction("IndexLogin", "Alert");
            }
            TempData["Save_Record"] = "Something went wrong..";
            TempData["URL"] = "/Login/ShowList";
            TempData["icon"] = "error";
            TempData["Time"] = "2000";
            return RedirectToAction("IndexLogin", "Alert");
        }
        [HttpGet]
        public IActionResult GetResterForm()
        {
            return View(new RegisterUser());
        }

        [HttpGet]
        public async Task<IActionResult> GetSubAdim(string role, string department)
        {
            var AllUserList = await _UserRepo.GetExecutive("GetExecutive");

            if (role == "Sub Admin" && department == "All")
            {
                var SubAdminList = AllUserList
                    .Where(item => item.Role == "Admin" && item.IsActive == true && item.IsApproved == true && item.IsDeleted == null)
                    .Select(item => new SelectListItem { Value = item.ID.ToString(), Text = item.FirstName + " " + item.LastName })
                    .ToList();

                var result = new
                {
                    SubAdminList = SubAdminList
                };
                return Json(result);
            }
            if (role == "Team Lead" && (department != "Sub Admin" || department == "Sub Admin"))
            {
                var ManagersList = AllUserList
                    .Where(item => item.Role == "Manager" && item.IsActive == true && item.IsApproved == true && item.IsDeleted == null && item.Department == department)
                    .Select(item => new SelectListItem { Value = item.ID.ToString(), Text = item.FirstName + " " + item.LastName })
                    .ToList();

                var result = new
                {
                    ManagersList = ManagersList
                };
                return Json(result);
            }
            else if (role == "Manager" && (department != "Sub Admin" || department == "Sub Admin"))
            {
                var SubAdminList = AllUserList
                    .Where(item => item.Role == "Sub Admin" && item.IsActive == true && item.IsApproved == true && item.IsDeleted == null)
                    .Select(item => new SelectListItem { Value = item.ID.ToString(), Text = item.FirstName + " " + item.LastName })
                    .ToList();

                var result = new
                {
                    SubAdminList = SubAdminList
                };
                return Json(result);
            }
            else if (role == "Executive" && (department != "Sub Admin" || department == "Sub Admin"))
            {
                var ManagersList = AllUserList
                    .Where(item => item.Role == "Manager" && item.IsActive == true && item.IsApproved == true && item.IsDeleted == null && item.Department == department)
                    .Select(item => new SelectListItem { Value = item.ID.ToString(), Text = item.FirstName + " " + item.LastName })
                    .ToList();

                var TLList = AllUserList.Where(item => item.Role == "Team Lead" && item.IsActive == true && item.IsApproved == true && item.IsDeleted == null)
                       .Select(item => new SelectListItem { Value = item.ID.ToString(), Text = item.FirstName + " " + item.LastName }).ToList();


                var result = new
                {
                    ManagersList = ManagersList
                };
                return Json(result);
            }
            return Json(new { SubAdminList = new List<SelectListItem>(), ManagersList = new List<SelectListItem>() });
        }

        [HttpGet]
        public async Task<IActionResult> GetTLS(string role, string department, string Manager)
        {
            var AllUserList = await _UserRepo.GetExecutive("GetExecutive");

            if (role != "Team Lead" && department != "" && Manager != "")
            {
                var SubAdminList = AllUserList
                    .Where(item => item.Role == "Team Lead" && item.Department == department && item.RelationID == Manager && item.IsActive == true && item.IsApproved == true && item.IsDeleted == null)
                    .Select(item => new SelectListItem { Value = item.ID.ToString(), Text = item.FirstName + " " + item.LastName })
                    .ToList();

                var result = new
                {
                    TlsList = SubAdminList
                };
                return Json(result);
            }
            return Json(new { SubAdminList = new List<SelectListItem>(), ManagersList = new List<SelectListItem>() });
        }

        [HttpGet]
        public async Task<IActionResult> GetFulluser(string userId)
        {
            var AllUserList = await _UserRepo.GetExecutive("GetExecutive");

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
            {
                var user = AllUserList.FirstOrDefault(u => u.ID == userIdInt);
                if (user == null)
                {
                    return Json(new { error = "User not found" });
                }

                // Initialize all variables
                var executive = user.Role == "Executive" ? user : null;
                var teamLead = user.Role == "Team Lead" ? user : null;
                var manager = user.Role == "Manager" ? user : null;
                var subAdmin = user.Role == "Sub Admin" ? user : null;

                // Build hierarchy upwards based on current role
                if (executive != null)
                {
                    teamLead = AllUserList.FirstOrDefault(u => u.ID == Convert.ToUInt32(executive.RelationID) && u.Role == "Team Lead");
                    manager = teamLead != null
                        ? AllUserList.FirstOrDefault(u => u.ID == Convert.ToUInt32(teamLead.RelationID) && u.Role == "Manager")
                        : AllUserList.FirstOrDefault(u => u.ID == Convert.ToUInt32(executive.RelationID) && u.Role == "Manager");
                    subAdmin = manager != null
                        ? AllUserList.FirstOrDefault(u => u.ID == Convert.ToUInt32(manager.RelationID) && u.Role == "Sub Admin")
                        : null;
                }
                else if (teamLead != null)
                {
                    manager = AllUserList.FirstOrDefault(u => u.ID == Convert.ToUInt32(teamLead.RelationID) && u.Role == "Manager");
                    subAdmin = manager != null
                        ? AllUserList.FirstOrDefault(u => u.ID == Convert.ToUInt32(manager.RelationID) && u.Role == "Sub Admin")
                        : null;
                }
                else if (manager != null)
                {
                    subAdmin = AllUserList.FirstOrDefault(u => u.ID == Convert.ToUInt32(manager.RelationID) && u.Role == "Sub Admin");
                }

                var result = new
                {
                    ExecutiveId = executive?.ID,
                    ExecutiveName = executive != null ? $"{executive.FirstName} {executive.LastName}" : null,

                    TeamLeadId = teamLead?.ID,
                    TeamLeadName = teamLead != null ? $"{teamLead.FirstName} {teamLead.LastName}" : null,

                    ManagerId = manager?.ID,
                    ManagerName = manager != null ? $"{manager.FirstName} {manager.LastName}" : null,

                    SubAdminId = subAdmin?.ID,
                    SubAdminName = subAdmin != null ? $"{subAdmin.FirstName} {subAdmin.LastName}" : null
                };

                return Json(result);
            }

            return Json(new { error = "Valid user ID is required" });
        }


        [HttpGet]
        public async Task<IActionResult> GetALLData(string role, string department)
        {
            var AllUserList = await _UserRepo.GetExecutive("GetExecutive");

            // Universal: Filter only active, approved, non-deleted users
            AllUserList = AllUserList
                .Where(u => u.IsActive == true && u.IsApproved == true && u.IsDeleted == null)
                .ToList();

            var subAdminList = AllUserList
                .Where(u => u.Role == "Sub Admin")
                .Select(u => new SelectListItem { Value = u.ID.ToString(), Text = $"{u.FirstName} {u.LastName}" })
                .ToList();

            if (role == "Sub Admin" && department == "All")
            {
                // Sub Admin registering, show only Admins
                var adminList = AllUserList
                    .Where(u => u.Role == "Admin")
                    .Select(u => new SelectListItem { Value = u.ID.ToString(), Text = $"{u.FirstName} {u.LastName}" })
                    .ToList();

                return Json(new { SubAdminList = adminList });
            }

            if (role == "Team Lead")
            {
                var managersList = AllUserList
                    .Where(u => u.Role == "Manager" && u.Department == department)
                    .Select(u => new SelectListItem { Value = u.ID.ToString(), Text = $"{u.FirstName} {u.LastName}" })
                    .ToList();

                return Json(new
                {
                    ManagersList = managersList,
                    SubAdminList = subAdminList
                });
            }

            if (role == "Manager")
            {
                return Json(new { SubAdminList = subAdminList });
            }

            if (role == "Executive")
            {
                var managersList = AllUserList
                    .Where(u => u.Role == "Manager" && u.Department == department)
                    .Select(u => new SelectListItem { Value = u.ID.ToString(), Text = $"{u.FirstName} {u.LastName}" })
                    .ToList();

                var tlList = AllUserList
                    .Where(u => u.Role == "Team Lead" && u.Department == department)
                    .Select(u => new SelectListItem { Value = u.ID.ToString(), Text = $"{u.FirstName} {u.LastName}" })
                    .ToList();

                return Json(new
                {
                    ManagersList = managersList,
                    TlsList = tlList,
                    SubAdminList = subAdminList
                });
            }

            return Json(new { SubAdminList = new List<SelectListItem>(), ManagersList = new List<SelectListItem>() });
        }


    }

}