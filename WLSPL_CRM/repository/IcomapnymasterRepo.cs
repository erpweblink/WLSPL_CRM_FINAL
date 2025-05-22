using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public interface IcomapnymasterRepo
    {
        Task<List<Company>> Getcompcode(string Action);

        Task<int> SubmitDetails(Company Model, string Action);

        Task<List<Company>> checkcomapnies(string action, string company, string GstNo);

        Task<List<Company>> GetLeadlist(string Action, Company Model);

        Task<dynamic> GetcompanybyId(string Id);

        Task<int> DeleteReord(string ID, String CreatedBy);

        Task<List<Company>> SearchCompanyAsync(string Action);
    }
}
