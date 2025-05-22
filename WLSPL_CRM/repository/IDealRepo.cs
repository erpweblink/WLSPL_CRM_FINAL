using System.Collections.Generic;
using System.Threading.Tasks;
using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public interface IDealRepo
    {
        Task<List<Deal>> GetAllDeals();
        Task<int> SubmitDetails(Deal model, string Action);
    }
}
