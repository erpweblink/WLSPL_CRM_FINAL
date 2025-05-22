using Dapper;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;
using System.Data;
using WLSPL_CRM_2.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static WLSPL_CRM_2.Models.Quatation;

namespace WLSPL_CRM_2.repository
{
    public class Quotationrepo : IQuotationRepo
    {
        private readonly IConfiguration _configuration;
        public Quotationrepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<Quatation>> checkcompaniesdetails(string action, String Company)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", action);
                parameters.Add("@CompanyName", Company);
                var result = await connection.QueryAsync<Quatation>(
                    "SP_companymaster",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }
        public async Task<List<Quatation>> GetQuotationlist(string Action, Quatation Model)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", Action);
                var result = await connection.QueryAsync<Quatation>(
                    "SP_Quotation",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }
        public async Task<dynamic> GetQutationbyId(string Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", Id);
                parameters.Add("@Action", "GetDetailsbyID");
                var result = await connection.QueryFirstOrDefaultAsync<QuotationDto>(
                    "SP_Quotation",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return result;
            }
        }
        public async Task<dynamic> GetQutationDetailsbyId(string Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", Id);
                parameters.Add("@Action", "GetQuotationDetailsbyID");
                var result = await connection.QueryAsync<Quatation, QuotationItem, Quatation>(
                  "SP_Quotation",
                  (quotation, item) =>
                  {
             
                      if (quotation.Items == null)
                      {
                          quotation.Items = new List<QuotationItem>();
                      }

              
                      quotation.Items.Add(item);

               
                      return quotation;
                  },
                  param: parameters,
                  splitOn: "Item", 
                  commandType: CommandType.StoredProcedure
              );

      
                var groupedResult = result
                    .GroupBy(q => q.HeaderID) 
                    .Select(g =>
                    {
                        var quotation = g.First(); 
                        quotation.Items = g.SelectMany(q => q.Items).ToList();
                        quotation.ItemCount = quotation.Items.Count; 
                        return quotation;
                    })
                    .FirstOrDefault(); 

                return groupedResult;

            }
        }
        public async Task<int?> SubmitDetails(Quatation Model, string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", Model.ID);
                parameters.Add("@LeadCode", Model.LeadCode);
                parameters.Add("@Companyname", Model.Companyname);
                parameters.Add("@Gstno", Model.Gstno);
                parameters.Add("@Quotationdate", Model.Quotationdate);
                parameters.Add("@Validdate", Model.Validdate);
                parameters.Add("@Mobileno", Model.Mobileno);
                parameters.Add("@Emailid", Model.Emailid);
                parameters.Add("@Billingaddress", Model.Billingaddress);
                parameters.Add("@Shippingaddress", Model.Shippingaddress);
                parameters.Add("@Action", Action);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_Quotation", parameters, commandType: CommandType.StoredProcedure);
                int? ID = parameters.Get<int?>("@Result");
                if (Action != "UpdateHeaderDetails")
                {
                    if (ID != null)
                    {
                        foreach (var item in Model.Items)
                        {
                            await InsertItemAsync(connection, item, Model.Companyname, ID.Value);
                        }
                    }
                }
                else
                {
                    if (ID != null)
                    {
                        await deleteItemDetails(connection ,ID.Value);
                        foreach (var item in Model.Items)
                        {
                            await UpdateItemAsync(connection, item, Model.Companyname, ID.Value);
                        }
                    }
                }

              
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;

            }
        }
        private async Task InsertItemAsync(SqlConnection connection, QuotationItem item, string companyName, int headerId)
        {
            var itemParameters = new DynamicParameters();
            itemParameters.Add("@HeaderID", headerId);
            itemParameters.Add("@Item", item.Item);
            itemParameters.Add("@Hsn", item.Hsn);
            itemParameters.Add("@Qty", item.Qty);
            itemParameters.Add("@Price", item.Price);
            itemParameters.Add("@Discount", item.Discount);
            itemParameters.Add("@DiscountAmount", item.DiscountAmount);
            itemParameters.Add("@Tax", item.Tax);
            itemParameters.Add("@TaxAmount", item.TaxAmount);
            itemParameters.Add("@Amount", item.Amount);
            itemParameters.Add("@Action", "InsertItemdeatils");
            itemParameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync("SP_Quotation", itemParameters, commandType: CommandType.StoredProcedure);
            int? isSuccess1 = itemParameters.Get<int?>("@Result");
        }
        private async Task UpdateItemAsync(SqlConnection connection, QuotationItem item, string companyName, int headerId)
        {
            var itemParameters = new DynamicParameters();
            itemParameters.Add("@HeaderID", headerId);
            itemParameters.Add("@Item", item.Item);
            itemParameters.Add("@Hsn", item.Hsn);
            itemParameters.Add("@Qty", item.Qty);
            itemParameters.Add("@Price", item.Price);
            itemParameters.Add("@Discount", item.Discount);
            itemParameters.Add("@DiscountAmount", item.DiscountAmount);
            itemParameters.Add("@Tax", item.Tax);
            itemParameters.Add("@TaxAmount", item.TaxAmount);
            itemParameters.Add("@Amount", item.Amount);
            itemParameters.Add("@Action", "UpdateItemsDetails");
            itemParameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync("SP_Quotation", itemParameters, commandType: CommandType.StoredProcedure);
            int? isSuccess1 = itemParameters.Get<int?>("@Result");
        }
        private async Task deleteItemDetails(SqlConnection connection, int headerId)
        {
            var itemParameters = new DynamicParameters();
            itemParameters.Add("@HeaderID", headerId);
            itemParameters.Add("@Action", "DeleteItemdetails");
            itemParameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync("SP_Quotation", itemParameters, commandType: CommandType.StoredProcedure);
            int? isSuccess2 = itemParameters.Get<int?>("@Result");
        }
        public async Task<int> DeleteQuotation(string ID, string CreatedBy)
        {
            using(var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", ID);
                parameters.Add("@Action", "DeleteQuotation");
                parameters.Add("@createdby", CreatedBy);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_Quotation", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }

        public  async Task<List<Quatation>> GetservicesDetails(string action, string Department)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", action);
                parameters.Add("@Department", Department);
                var result = await connection.QueryAsync<Quatation>(
                    "SP_Quotation",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }
    }
}
