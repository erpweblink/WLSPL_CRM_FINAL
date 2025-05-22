using System.Collections.Generic;
using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public interface IleadRepo
    {
        Task<List<LeadGenration>> GetLeadcode(string Action);
        Task<int>SubmitDetails(LeadGenration Model);
        Task<int> UpdateDetails(LeadGenration Model);
        Task<List<LeadGenration>> GetLeadlist(string Action, LeadGenration Model,string role,string ID);
        Task<dynamic> GetleadbyId(string Id);
        Task<List<LeadGenration>>Getcompanies(string Action);
        Task<int> DeleteReord(string ID, String CreatedBy);
        Task<int>submitconnetciondetails (LeadGenration Model);

    }
}
