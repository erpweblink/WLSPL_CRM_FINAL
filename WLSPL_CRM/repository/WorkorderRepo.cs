
using Dapper;
using Microsoft.Data.SqlClient;
using static WLSPL_CRM_2.Models.Quatation;
using System.Data;
using WLSPL_CRM_2.Models;
using static WLSPL_CRM_2.Models.Workorder;

namespace WLSPL_CRM_2.repository
{
    public class WorkorderRepo : IWorkorderRepo
    {

        private readonly IConfiguration _configuration;
        public WorkorderRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<dynamic> GetQutationbyQuotaion(string Quotation)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@QuatationNo", Quotation);
                parameters.Add("@Action", "GetQuotationDetails");
                var result = await connection.QueryFirstOrDefaultAsync<Workorder>(
                    "SP_WorkOrderDetails",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<dynamic> GetQutationDetails(string Quotation)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@QuatationNo", Quotation);
                parameters.Add("@Action", "GetDetails");
                var result = await connection.QueryAsync<Workorder, QuotationItemDetails, Workorder>(
                  "SP_WorkOrderDetails",
                  (quotation, item) =>
                  {

                      if (quotation.Items == null)
                      {
                          quotation.Items = new List<QuotationItemDetails>();
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
                        //quotation.ItemCount = quotation.Items.Count;
                        return quotation;
                    })
                    .FirstOrDefault();

                return groupedResult;

            }
        }

        public async Task<dynamic> GetWorkorderNo(string Action)
        {

            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", "GetworkorderNo");
                var result = await connection.QueryFirstOrDefaultAsync<Workorder>(
                    "SP_WorkOrderDetails",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<int?> SubmitDetails(Workorder Model, string Action)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", Model.ID);
                parameters.Add("@CustomerName", Model.Companyname);
                parameters.Add("@WONO", Model.WONO);
                parameters.Add("@WOcreateDate", Model.WOcreateDate);
                parameters.Add("@GSTNO", Model.GSTNO);
                parameters.Add("@QuatationNo", Model.QuotationNo);
                parameters.Add("@BalanceAmount", Model.BalanceAmount);
                parameters.Add("@TotalFullamount", Model.TotalFullamount);
                parameters.Add("@grandTotal", Model.grandTotal);
                parameters.Add("@Action", Action);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_WorkOrderDetails", parameters, commandType: CommandType.StoredProcedure);
                int? ID = parameters.Get<int?>("@Result");
                if (Action != "UpdateHdrdetails")
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
                        await deleteItemDetails(connection, ID.Value);
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

        private async Task InsertItemAsync(SqlConnection connection, QuotationItemDetails item, string companyName, int headerId)
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
            await connection.ExecuteAsync("SP_WorkOrderDetails", itemParameters, commandType: CommandType.StoredProcedure);
            int? isSuccess1 = itemParameters.Get<int?>("@Result");
        }
        private async Task UpdateItemAsync(SqlConnection connection, QuotationItemDetails item, string companyName, int headerId)
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
            await connection.ExecuteAsync("SP_WorkOrderDetails", itemParameters, commandType: CommandType.StoredProcedure);
            int? isSuccess1 = itemParameters.Get<int?>("@Result");
        }
        private async Task deleteItemDetails(SqlConnection connection, int headerId)
        {
            var itemParameters = new DynamicParameters();
            itemParameters.Add("@HeaderID", headerId);
            itemParameters.Add("@Action", "DeleteItemdetails");
            itemParameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync("[DB_CRMERPWLSPL].[SP_WorkOrderDetails]", itemParameters, commandType: CommandType.StoredProcedure);
            int? isSuccess2 = itemParameters.Get<int?>("@Result");
        }
        public async Task<int> DeleteQuotation(string ID, string CreatedBy)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", ID);
                parameters.Add("@Action", "DeleteQuotation");
                parameters.Add("@createdby", CreatedBy);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("SP_WorkOrderDetails", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }

        public async Task<List<Workorder>> GetWorkorderList(string Action, Workorder Model)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", Action);
                var result = await connection.QueryAsync<Workorder>(
                    "[DB_CRMERPWLSPL].[SP_WorkOrderDetails]",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }

        public async Task<dynamic> GetbyWorkdetailsbyId(string Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", Id);
                parameters.Add("@Action", "GetWoDetailsbyID");
                var result = await connection.QueryFirstOrDefaultAsync<Workorder>(
                    "[DB_CRMERPWLSPL].[SP_WorkOrderDetails]",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<dynamic> GetWorkItemDetailsbyId(string Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", Id);
                parameters.Add("@Action", "GetWoItemDetailsbyID");
                var result = await connection.QueryAsync<Workorder, QuotationItemDetails, Workorder>(
                  "[DB_CRMERPWLSPL].[SP_WorkOrderDetails]",
                  (quotation, item) =>
                  {

                      if (quotation.Items == null)
                      {
                          quotation.Items = new List<QuotationItemDetails>();
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
                 
                        return quotation;
                    })
                    .FirstOrDefault();

                return groupedResult;

            }
        }

        public async Task<int> DeleteWorkorder(string ID, string CreatedBy)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Conn_String")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", ID);
                parameters.Add("@Action", "Deleteworkorder");
                parameters.Add("@createdby", CreatedBy);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("[DB_CRMERPWLSPL].[SP_WorkOrderDetails]", parameters, commandType: CommandType.StoredProcedure);
                int isSuccess = parameters.Get<int>("@Result");
                return isSuccess;
            }
        }
    }
}
