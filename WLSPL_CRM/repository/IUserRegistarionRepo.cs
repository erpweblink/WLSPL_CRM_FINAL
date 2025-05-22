using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WLSPL_CRM.Models;
using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public interface IUserRegistrationRepo
    {
        Task<List<RegisterUser>> GetExecutive(string action);

        Task<List<RegisterUser>> SearchExecutiveAsync(string name);
        Task<int> RegisterUser(RegisterUser user);
        Task<dynamic> LoginUser(string userName, string password);
        Task<dynamic> ForgetUser(string userName, string Email);
        Task<List<RegisterUser>> GetpendingRequest(string action);
        Task<dynamic> GetUserById(string Id);
        Task<int> UpdateDeatils(string userRole, string userdepartment, int ID, bool IsApproved, bool IsActive, string Relationid);
        Task<int> InsertForgetData(ForgetPasswordMaster obj);
        Task<int> DeleteForgetData(string MailID);
        Task<dynamic> GetForgetPasswordRequest(string userId, string UserMail);
        Task<int> UpdateNewpassword(string Password, string userId, string UserMail, string ConfirmPass);
        Task<int> UpdateUserDetails(RegisterUser User);
        Task<int> NotapprovedRecords(string Action);


        Task<List<UserViewModel>> UserListOrg(string Action);
    }
}