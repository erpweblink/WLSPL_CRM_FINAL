using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WLSPL_CRM_2.Controllers;
using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public class prospectrepo : Iprospectrepo
    {

        private readonly IConfiguration _configuration;

        public prospectrepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public  async Task<int> CancelPartycount()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();

                var count = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(ID) AS PendingCount " +
                "FROM [DB_CRMERPWLSPL].[Tblprospect] " +
                "WHERE Type = 'prospect' AND stage = 'Cancel Party';",
                commandType: CommandType.Text
            );
                return count;
            }
            
        }

        public async Task<int> Discussioncount()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();

                var count = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(ID) AS PendingCount " +
                "FROM [DB_CRMERPWLSPL].[Tblprospect] " +
                "WHERE Type = 'prospect' AND stage = 'Discussion';",
                commandType: CommandType.Text
            );
                return count;
            }
        }

        public async Task<int> Finalizecount()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();

                var count = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(ID) AS PendingCount " +
                "FROM [DB_CRMERPWLSPL].[Tblprospect] " +
                "WHERE Type = 'prospect' AND stage = 'Finalize';",
                commandType: CommandType.Text
            );
                return count;
            }
        }

        public async Task<List<Prospect>> GetProspectslsit(string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                try
                {
                    await connection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", Action);
                    var result = await connection.QueryAsync<Prospect>(
                        "SP_Prospect",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.ToList();
                }
                catch (Exception ex)
                {

                    throw new ApplicationException("An error occurred while retrieving the prospects.", ex);
                }
            }
        }

        public async Task<int> pendingcount()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();

                var count = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(ID) AS PendingCount " +
                "FROM [DB_CRMERPWLSPL].[Tblprospect] " +
                "WHERE Type = 'prospect' AND stage = 'Pending Lead';",
                commandType: CommandType.Text
            );
                return count; 
            }
        }

      
    }
}

