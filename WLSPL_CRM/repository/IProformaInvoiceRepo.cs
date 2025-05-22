using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public interface IProformaInvoiceRepo
    {
        Task<List<Company>> checkcompaniesdetails(string action); 
        Task<Company> GetCustdetails(string action, string CompnayName); 
        Task<List<Quatation>> checkquotationdetails(string action,string CompnayName);
        Task<dynamic> Getquotationdetails(string action,string QuotationParam);
        Task<int> SubmitDetails(ProformaInvoiceHdr Model, string Action);
        Task<List<ProformaInvoiceHdr>> GetProformalist(string Action);
        Task<dynamic> GetProformabyId(string action, string Id);
        Task<string> GetProformaNumber(string Fn_Name);
        Task<List<Services>> GetServiceList(string Action);
        Task<Services> GetServiceByID(string Action,string ProductId);
    }
}
