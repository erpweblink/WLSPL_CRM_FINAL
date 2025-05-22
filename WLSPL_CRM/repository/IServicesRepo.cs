using WLSPL_CRM_2.Models;
using System.Collections.Generic;

namespace WLSPL_CRM_2.repository
{
    public interface IServicesRepo
    {
        Task<List<Services>> GetDepartment(string Action);
        Task<int?> Submitservices(Services Model, string Action);
        Task<int> Delete(string ID, String CreatedBy);

    }
}
