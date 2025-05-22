using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public interface IWorkorderRepo
    {
        Task<dynamic> GetQutationbyQuotaion(string Quotation);
        Task<dynamic> GetQutationDetails(string Quotation);
        Task<dynamic> GetWorkorderNo(string Action);
        Task<int?> SubmitDetails(Workorder Model, string Action);
        Task<List<Workorder>> GetWorkorderList(string Action, Workorder Model);
        Task<dynamic> GetbyWorkdetailsbyId(string Id);
        Task<dynamic> GetWorkItemDetailsbyId(string Id);
        Task<int> DeleteWorkorder(string ID, String CreatedBy);

    }
}
