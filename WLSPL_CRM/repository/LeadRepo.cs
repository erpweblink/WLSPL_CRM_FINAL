
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;
using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public class LeadRepo : IleadRepo
    {
        private readonly IConfiguration _configuration;

        public LeadRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<LeadGenration>> Getcompanies(string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", Action);
                var result = await connection.QueryAsync<LeadGenration>(
                    "SP_Lead",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return result.ToList();

            }
        }

        public async Task<dynamic> GetleadbyId(string Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", Id);
                parameters.Add("@Action", "GetLeadbyId");
                var result = await connection.QueryFirstOrDefaultAsync<LeadGenration>(
                    "SP_Lead",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<List<LeadGenration>> GetLeadcode(string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@Action", Action);
                var result = await connection.QueryAsync<LeadGenration>(
                    "SP_Lead",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result.ToList();

            }
        }

        public async Task<List<LeadGenration>> GetLeadlist(string Action, LeadGenration Model,string role,string ID)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", Action);
                parameters.Add("@Role", role);
                parameters.Add("@id", Convert.ToInt32(ID));
                var result = await connection.QueryAsync<LeadGenration>(
                    "SP_Lead",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return result.ToList();

            }
        }

        public async Task<int> SubmitDetails(LeadGenration Model)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Leadcode", Model.Leadcode);
                parameters.Add("@CompanyName", Model.CompanyName);                         
                parameters.Add("@Email", Model.Email);
                parameters.Add("@Mobile", Model.Mobile);              
                parameters.Add("@City", Model.City);               
                parameters.Add("@Source", Model.Source); 
                parameters.Add("@Requirements", Model.Requirements);       
                parameters.Add("@Action", "Insert");
                parameters.Add("@Createdby", Model.Createdby);
                parameters.Add("@CreatedOn", DateTime.Now);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_Lead", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }

        public async Task<int> UpdateDetails(LeadGenration Model)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("ID", Model.ID);
                parameters.Add("@Leadcode", Model.Leadcode);
                parameters.Add("@CompanyName", Model.CompanyName);               
                parameters.Add("@Email", Model.Email);
                parameters.Add("@Mobile", Model.Mobile);         
                parameters.Add("@City", Model.City);         
                parameters.Add("@Source", Model.Source);   
                parameters.Add("@Requirements", Model.Requirements);           
                parameters.Add("@Action", "UpdateDetails");
                parameters.Add("@UpdatedBy", Model.Createdby);
                parameters.Add("@UpdatedOn", DateTime.Now);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_Lead", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }

        public async Task<int> DeleteReord(string ID, string CreatedBy)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("ID", ID);
                parameters.Add("@Action", "DeleteRecord");
                parameters.Add("@DeletedBy", CreatedBy);
                parameters.Add("@DeletedOn", DateTime.Now);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_Lead", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }

        public async Task<int> submitconnetciondetails(LeadGenration Model)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id ", Model.ID);
                parameters.Add("@Action", "SetApproval");
                parameters.Add("@Executive", Model.UserID);
                parameters.Add("@Approveddate", Model.CreatedOn);
                parameters.Add("@IsApproved", 1);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_Lead", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }

    }
}
