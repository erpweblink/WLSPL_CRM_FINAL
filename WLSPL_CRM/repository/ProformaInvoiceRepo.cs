using System;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public class ProformaInvoiceRepo : IProformaInvoiceRepo
    {
        private readonly IConfiguration _configuration;
        public ProformaInvoiceRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<Company>> checkcompaniesdetails(string action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", action);
                var result = await connection.QueryAsync<Company>(
                    "SP_ProformaInvoice",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }
        public async Task<List<Quatation>> checkquotationdetails(string action, string CompnayName)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", action);
                parameters.Add("@companyname", CompnayName);
                var result = await connection.QueryAsync<Quatation>(
                    "SP_ProformaInvoice",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }
        public async Task<dynamic> Getquotationdetails(string action, string QuotationParam)
        {
            if (action == "GETQuotationData")
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
                {
                    await connection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", action);
                    parameters.Add("@companyname", QuotationParam);
                    var result = await connection.QueryAsync<Quatation>(
                        "SP_ProformaInvoice",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    return result.FirstOrDefault();
                }
            }
            else
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
                {
                    await connection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", action);
                    parameters.Add("@companyname", QuotationParam);
                    var result = await connection.QueryAsync<Quatation.QuotationItem>(
                        "SP_ProformaInvoice",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    return result.ToList();
                }
            }

        }
        public async Task<dynamic> GetProformabyId(string action, string Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                if (action == "GetProformaByID")
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", action);
                    parameters.Add("@id ", Id);
                    var result = await connection.QueryAsync<ProformaInvoiceHdr>(
                        "SP_ProformaInvoice",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    return result.FirstOrDefault();
                }
                else
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", action);
                    parameters.Add("@id ", Id);
                    var result = await connection.QueryAsync<ProformaInvoiceDtls>(
                        "SP_ProformaInvoice",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    return result.ToList();
                }

            }
        }

        public async Task<List<ProformaInvoiceHdr>> GetProformalist(string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@action", Action);
                var result = await connection.QueryAsync<ProformaInvoiceHdr>(
                    "SP_ProformaInvoice",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }

        public async Task<int> SubmitDetails(ProformaInvoiceHdr model, string Action)
        {
            try
            {
                if(Action == "DeleteProforma"){
                    using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
                    {
                        await connection.OpenAsync();
                        var parameters = new DynamicParameters();
                        parameters.Add("@id", model.Id);
                        parameters.Add("@IsDeleted", true);
                        parameters.Add("@Action", Action);
                        parameters.Add("@returnId", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);
                        await connection.ExecuteAsync("SP_ProformaInvoice", parameters, commandType: CommandType.StoredProcedure);
                        string returnIdStr = parameters.Get<string>("@returnId");
                        int? ID = int.TryParse(returnIdStr, out var parsedId) ? parsedId : null;
                        int isSuccess = ID ?? 0;
                        return isSuccess;
                    }
                }
                else{
                    using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
                    {
                        await connection.OpenAsync();
                        var parameters = new DynamicParameters();
                        parameters.Add("@proformano", model.ProformaNo);
                        parameters.Add("@proformadate", model.ProformaDate);
                        parameters.Add("@reversecharge", model.ReverseCharge?.ToString());
                        parameters.Add("@state", model.State);
                        parameters.Add("@companyname", model.CompanyName);
                        parameters.Add("@address", model.Address);
                        parameters.Add("@cgstin", model.GstNo);
                        parameters.Add("@AllAmt", model.AllProTotalAmount?.ToString());
                        parameters.Add("@quotationNo", model.QuotationNo);
                        parameters.Add("@AgainstBy", model.AgainstBy);
                        parameters.Add("@Action", Action);
                        if (Action == "UpdateHeaderDetails")
                        {
                            parameters.Add("@id", model.Id);
                        }
                        parameters.Add("@sessionname", model.CreatedBy);
                        parameters.Add("@returnId", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);
                        await connection.ExecuteAsync("SP_ProformaInvoice", parameters, commandType: CommandType.StoredProcedure);
                        string returnIdStr = parameters.Get<string>("@returnId");
                        int? ID = int.TryParse(returnIdStr, out var parsedId) ? parsedId : null;
                        if (Action != "UpdateHeaderDetails")
                        {
                            if (ID != null)
                            {
                                foreach (var item in model.InvoiceDetails)
                                {
                                    await InsertItemAsync(connection, item, ID.Value);
                                }
                            }
                        }
                        else
                        {
                            if (ID != null)
                            {
                                await deleteItemDetails(connection, ID.Value);
                                foreach (var item in model.InvoiceDetails)
                                {
                                    await InsertItemAsync(connection, item, ID.Value);
                                }
                            }
                        }


                        int isSuccess = ID ?? 0;
                        return isSuccess;
                    }
                }
                    
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task InsertItemAsync(SqlConnection connection, ProformaInvoiceDtls item, int headerId)
        {
            var itemParameters = new DynamicParameters();
            itemParameters.Add("@ProformaId", headerId);
            itemParameters.Add("@ProductDescription", item.ProductDescription);
            itemParameters.Add("@SACCode", item.SACCode);
            itemParameters.Add("@Qty", item.Qty);
            itemParameters.Add("@Rate", item.Rate);
            itemParameters.Add("@Amount", item.Total);
            itemParameters.Add("@Discount", item.Discount);
            itemParameters.Add("@DiscountAmount", item.DiscountAmt);
            itemParameters.Add("@Tax", item.Tax);
            itemParameters.Add("@TaxAmount", item.TaxAmt);
            itemParameters.Add("@GrandTotal", item.GrandTotal);
            itemParameters.Add("@Action", "InsertItemdeatils");
            itemParameters.Add("@returnId", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);
            await connection.ExecuteAsync("SP_ProformaInvoice", itemParameters, commandType: CommandType.StoredProcedure);
            string returnIdStr = itemParameters.Get<string>("@returnId");
            int? ID = int.TryParse(returnIdStr, out var parsedId) ? parsedId : null;
        }
        private async Task deleteItemDetails(SqlConnection connection, int headerId)
        {
            var itemParameters = new DynamicParameters();
            itemParameters.Add("@ProformaId", headerId);
            itemParameters.Add("@Action", "DeleteItemdetails");
            itemParameters.Add("@returnId", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);
            await connection.ExecuteAsync("SP_ProformaInvoice", itemParameters, commandType: CommandType.StoredProcedure);
            string returnIdStr = itemParameters.Get<string>("@returnId");
            int? ID = int.TryParse(returnIdStr, out var parsedId) ? parsedId : null;
        }

        public async Task<Company> GetCustdetails(string action, string CompnayName)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", action);
                parameters.Add("@companyname", CompnayName);
                var result = await connection.QueryAsync<Company>(
                    "SP_ProformaInvoice",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.FirstOrDefault();
            }
        }

        public async Task<string> GetProformaNumber(string Fn_Name)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var proformaNumber = await connection.ExecuteScalarAsync<string>("SELECT " + Fn_Name + " ");
                return proformaNumber;
            }
        }

        public async Task<List<Services>> GetServiceList(string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@action", Action);
                var result = await connection.QueryAsync<Services>(
                    "SP_ProformaInvoice",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }

        public async Task<Services> GetServiceByID(string Action, string ProductId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", Action);
                parameters.Add("@companyname", ProductId);
                var result = await connection.QueryAsync<Services>(
                    "SP_ProformaInvoice",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.FirstOrDefault();
            }
        }
    }
}
