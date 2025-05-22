using System.Collections.Generic;
using System.Threading.Tasks;
using WLSPL_CRM_2.Models;
namespace WLSPL_CRM_2.repository
{
    public interface Iprospectrepo
    {
        Task<List<Prospect>> GetProspectslsit(string Action);
        Task<int> pendingcount();
        Task<int> Discussioncount();
        Task<int> Finalizecount();
        Task<int> CancelPartycount();
    }
}
