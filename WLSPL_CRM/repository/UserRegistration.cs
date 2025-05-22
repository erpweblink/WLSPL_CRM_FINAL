using System;
using System.Data;
using System.Net;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using WLSPL_CRM.Models;
using WLSPL_CRM_2.Models;
using static System.Net.Mime.MediaTypeNames;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WLSPL_CRM_2.repository
{
    public class UserRegistration : IUserRegistrationRepo
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRegistration(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<dynamic> LoginUser(string userName, string password)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@Username", userName);
                parameters.Add("@password", password);
                parameters.Add("@Action", "Logindata");
                var result = await connection.QueryFirstOrDefaultAsync<RegisterUser>(
                    "SP_User",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }
        public async Task<int> RegisterUser(RegisterUser user)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
                {
                    await connection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@FirstName", user.FirstName);
                    parameters.Add("@MiddleName", user.MiddleName);
                    parameters.Add("@LastName", user.LastName);
                    parameters.Add("@UserName", user.UserName);
                    parameters.Add("@Email_ID", user.EmailId);
                    parameters.Add("@Password", user.Password);
                    parameters.Add("@ConfirmPassword", user.ConfirmPassword);
                    parameters.Add("@MobileNo", user.MobileNo);
                    if (user.File != null && user.File.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "UserPhotos");
                        Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(user.File.FileName)}";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await user.File.CopyToAsync(fileStream);
                        }

                        user.PhotoPath = "/UserPhotos/" + uniqueFileName;
                    }
                    parameters.Add("@PhotoPath", user.PhotoPath);
                    parameters.Add("@Action", "Insert");
                    parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await connection.ExecuteAsync("SP_User", parameters, commandType: CommandType.StoredProcedure);
                    int isSuccess = parameters.Get<int>("@Result");
                    return isSuccess;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<List<RegisterUser>> GetpendingRequest(string action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                try
                {
                    await connection.OpenAsync();
                    var parameters = new { Action = action };
                    var result = await connection.QueryAsync<RegisterUser>(
                        "[SP_User]",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.ToList();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error retrieving pending requests", ex);
                }
            }
        }
        public async Task<dynamic> GetUserById(string Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", Id);
                parameters.Add("@Action", "GetRequestbyId");
                var result = await connection.QueryFirstOrDefaultAsync<RegisterUser>(
                    "SP_User",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return result;
            }
        }
        public async Task<int> UpdateDeatils(string userRole, string userdepartment, int ID, bool IsApproved, bool IsActive, string Relationid)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@userdepartment", userdepartment);
                parameters.Add("@userRole", userRole);
                parameters.Add("@RelatedId", Relationid);
                if (IsActive)
                {
                    parameters.Add("@IsActive", IsActive);
                }
                if (IsApproved)
                {
                    parameters.Add("@IsApproved", IsApproved);
                }
                parameters.Add("@id", ID);
                parameters.Add("@Action", "UpdateDetails");
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_User", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }

        // Code by Nikhil for forget functionality 09-05-2025
        public async Task<dynamic> ForgetUser(string userName, string Email)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@Username", userName);
                parameters.Add("@password", Email);
                parameters.Add("@Action", "ForgetPassword");
                var result = await connection.QueryFirstOrDefaultAsync<RegisterUser>(
                    "SP_User",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }
        public async Task<int> InsertForgetData(ForgetPasswordMaster obj)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@UserName", obj.userId);
                parameters.Add("@Email_ID", obj.userMail);
                parameters.Add("@Password", obj.Tempotp);
                parameters.Add("@Action", "InsertForgetOTP");
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_User", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }
        public async Task<int> DeleteForgetData(string MailID)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Email_ID", MailID);
                parameters.Add("@Action", "DeleteOldRecord");
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_User", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }
        public async Task<dynamic> GetForgetPasswordRequest(string userId, string UserMail)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                try
                {
                    await connection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserName", userId);
                    parameters.Add("@Email_ID", UserMail);
                    parameters.Add("@Action", "GetLatestOTP");
                    var result = await connection.QueryAsync<ForgetPasswordMaster>(
                        "[SP_User]",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error retrieving pending requests", ex);
                }
            }
        }
        public async Task<int> UpdateNewpassword(string Password, string userId, string UserMail, string ConfirmPass)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@UserName", userId);
                parameters.Add("@Email_ID", UserMail);
                parameters.Add("@Password", Password);
                parameters.Add("@ConfirmPassword", ConfirmPass);
                parameters.Add("@Action", "UpdateNewPasswordDetails");
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_User", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }
        public async Task<List<RegisterUser>> GetExecutive(string action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                try
                {
                    await connection.OpenAsync();
                    var parameters = new { Action = action };
                    var result = await connection.QueryAsync<RegisterUser>(
                        "[SP_User]",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.ToList();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error retrieving pending requests", ex);
                }
            }
        }
        public async Task<List<RegisterUser>> SearchExecutiveAsync(string name)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@UserName", name);
                parameters.Add("@Action", "SearchExecutive");

                var result = await connection.QueryAsync<RegisterUser>(
                    "SP_User",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }
        public async Task<int> UpdateUserDetails(RegisterUser user)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@ID", user.ID);
                parameters.Add("@FirstName", user.FirstName);
                parameters.Add("@MiddleName", user.MiddleName);
                parameters.Add("@LastName", user.LastName);
                parameters.Add("@UserName", user.UserName);
                parameters.Add("@Email_ID", user.EmailId);
                parameters.Add("@Password", user.Password);
                parameters.Add("@ConfirmPassword", user.ConfirmPassword);
                parameters.Add("@MobileNo", user.MobileNo);

                if (user.File != null && user.File.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "UserPhotos");
                    Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(user.File.FileName)}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await user.File.CopyToAsync(fileStream);
                    }

                    user.PhotoPath = "/UserPhotos/" + uniqueFileName;
                }
                parameters.Add("@PhotoPath", user.PhotoPath);
                parameters.Add("@Action", "UpdateUserDetails");
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_User", parameters, commandType: CommandType.StoredProcedure);
                string FullName = string.IsNullOrWhiteSpace((user.FirstName ?? "") + (user.LastName ?? "")) ? "No Name" : $"{user.FirstName ?? ""} {user.LastName ?? ""}".Trim();
                string image = user?.PhotoPath?? "/assets/images/users/avatar-1.jpg".ToString();
                _httpContextAccessor.HttpContext.Session.SetString("Profile", image);
                _httpContextAccessor.HttpContext.Session.SetString("FullName", FullName);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }
        public async Task<int> NotapprovedRecords(string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", Action);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_User", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }

        public async Task<List<UserViewModel>> UserListOrg(string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", Action);

                var result = await connection.QueryAsync<UserViewModel>(
                    "SP_User",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }

        }
    }
}