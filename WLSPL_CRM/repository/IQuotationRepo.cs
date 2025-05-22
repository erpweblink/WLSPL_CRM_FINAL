using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
	public interface IQuotationRepo
	{
		Task<List<Quatation>> checkcompaniesdetails(string action, string Company);
        Task<int?> SubmitDetails(Quatation Model, string Action);
        Task<List<Quatation>> GetQuotationlist(string Action, Quatation Model);
        Task<dynamic> GetQutationbyId(string Id);
        Task<dynamic> GetQutationDetailsbyId(string Id);
        Task<int> DeleteQuotation(string ID, String CreatedBy);
        Task<List<Quatation>> GetservicesDetails(string action, string Department);
    }
}
