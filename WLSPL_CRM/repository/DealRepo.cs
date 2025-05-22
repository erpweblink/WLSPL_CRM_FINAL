using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public class DealRepo : IDealRepo
    {
        private readonly IConfiguration _configuration;

        public DealRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Fetch all deals
        public async Task<List<Deal>> GetAllDeals()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                //string query = "SELECT * FROM Deals";
                string query = "SELECT * FROM Deals ORDER BY CreatedOn DESC";

                var deals = await connection.QueryAsync<Deal>(query);
                return deals.ToList();
            }
        }

        // Insert or Update deal
        public async Task<int> SubmitDetails(Deal model, string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Id", model.Id);
                parameters.Add("@Company", model.Company);
                parameters.Add("@DealOwner", model.DealOwner);
                parameters.Add("@Stage", model.Stage);
                parameters.Add("@Probability", model.Probability);
                parameters.Add("@ContactFrom", model.ContactFrom);
                parameters.Add("@Email", model.Email);
                parameters.Add("@ExpectedRevenue", model.ExpectedRevenue);
                parameters.Add("@Amount", model.Amount);
                parameters.Add("@CreatedBy", model.CreatedBy);
                //parameters.Add("@CreatedOn", model.CreatedOn);
                parameters.Add("@Action", Action);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("SP_DealsMaster", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("@Result");
            }
        }
    }
}
