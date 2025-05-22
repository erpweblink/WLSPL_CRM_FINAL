using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public class CompanymasterRepo : IcomapnymasterRepo
    {
        private readonly IConfiguration _configuration;
        public CompanymasterRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }
      
        public async Task<List<Company>> checkcomapnies(string action, string company, string GstNo)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", action);
                parameters.Add("@CompanyName", company);
                parameters.Add("@GSTNo", GstNo);
                var result = await connection.QueryAsync<Company>(
                    "SP_companymaster",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }

        public async Task<int> DeleteReord(string ID, string CreatedBy)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", ID);
                parameters.Add("@Action", "DeleteCompany");
                parameters.Add("@Deletedby", CreatedBy);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_companymaster", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }

        public  async Task<dynamic> GetcompanybyId(string Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", Id);
                parameters.Add("@Action", "Getcomapnybyid");
                var result = await connection.QueryFirstOrDefaultAsync<Company>(
                    "SP_companymaster",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<List<Company>> Getcompcode(string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", Action);
                var result = await connection.QueryAsync<Company>(
                    "SP_Lead",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }

        public async Task<List<Company>> GetLeadlist(string Action, Company Model)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", Action);
                var result = await connection.QueryAsync<Company>(
                    "SP_companymaster",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }

        public async Task<int> SubmitDetails(Company Model, string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@CompanyCode", Model.CompanyCode);
                parameters.Add("@id", Model.Id);
                parameters.Add("@CompanyName", Model.CompanyName);
                parameters.Add("@Registertype", Model.Registerfor);
                parameters.Add("@AreaNAme", Model.AreaNAme);
                parameters.Add("@SupplyType", Model.supplytype);
                parameters.Add("@OwnerName", Model.OwnerName);
                parameters.Add("@GSTNo", Model.GSTNo);
                parameters.Add("@ShippLocation", Model.ShippLocation);
                parameters.Add("@BillLocation", Model.BillLocation);
                parameters.Add("@Billingpincode", Model.BillingPincode);
                parameters.Add("@shippingpincode", Model.ShippingPincode);
                //parameters.Add("@Designation", Model.Designation1);
                parameters.Add("@BillStateCode", Model.BillStateCode);
                parameters.Add("@ShippStateCode", Model.ShippStateCode);
                parameters.Add("@BillAddress", Model.BillingAddress);
                parameters.Add("@shippaddress", Model.ShippingAddress);
                parameters.Add("@EmailID", Model.EmailID);
                parameters.Add("@MobileNo", Model.MobileNo);
                parameters.Add("@Designation", Model.Designation);
                parameters.Add("@Name", Model.Name);
                parameters.Add("@EmailID2", Model.EmailID1);
                parameters.Add("@MobileNo2", Model.MobileNo1);
                parameters.Add("@Designation2", Model.Designation1);
                parameters.Add("@Name2", Model.Designation1);

                parameters.Add("@Action", Action);
                //parameters.Add("@Createdby", Model.Createdby);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_companymaster", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }

        public async Task<List<Company>> SearchCompanyAsync(string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@CompanyName", Action);
                parameters.Add("@Action", "SearchCompany");

                var result = await connection.QueryAsync<Company>(
                    "SP_companymaster",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }
    }
}

