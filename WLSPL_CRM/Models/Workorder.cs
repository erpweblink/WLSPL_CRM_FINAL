using System.ComponentModel.DataAnnotations;
using static WLSPL_CRM_2.Models.Quatation;

namespace WLSPL_CRM_2.Models
{
    public class Workorder
    {
        public List<QuotationItemDetails>? Items { get; set; }
        public String? ID { get; set; }
        public String? HeaderID { get; set; }
        public string? WONO { get; set; }
        public string? QuotationNo { get; set; }
        public string? CustomerName { get; set; }
        public string? Companyname { get; set; }
        
        public string? GSTNO { get; set; }
        public DateTime? WOcreateDate { get; set; }
        public bool? AgainstFullAmount { get; set; }
        public string? TotalFullamount { get; set; }
        public string? BalanceAmount { get; set; }
        public DateTime? CreatedON { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedON { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }

        public string ? grandTotal { get; set; }
        public class QuotationItemDetails
        {
          
            public string? Item { get; set; }
            public string? Hsn { get; set; }
            public int? Qty { get; set; }
            public decimal? Price { get; set; }
            public decimal? Discount { get; set; }
            public decimal? DiscountAmount { get; set; }
            public decimal? Tax { get; set; }
            public decimal? TaxAmount { get; set; }
            public decimal? Amount { get; set; }
        }
    }
    
}
