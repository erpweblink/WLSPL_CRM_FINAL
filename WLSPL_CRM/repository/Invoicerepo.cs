using WLSPL_CRM_2.Models;

namespace WLSPL_CRM_2.repository
{
    public class Invoicerepo : IInvoiceRepo
    {
        public Task<List<Quatation>> checkcompaniesdetails(string action, string Company)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteQuotation(string ID, string CreatedBy)
        {
            throw new NotImplementedException();
        }

        public Task<List<Quatation>> GetQuotationlist(string Action, Quatation Model)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetQutationbyId(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetQutationDetailsbyId(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Quatation>> GetservicesDetails(string action, string Department)
        {
            throw new NotImplementedException();
        }

        public Task<int?> SubmitDetails(Quatation Model, string Action)
        {
            throw new NotImplementedException();
        }
    }
}
