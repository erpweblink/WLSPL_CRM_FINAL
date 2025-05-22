using System.ComponentModel.DataAnnotations;

namespace WLSPL_CRM_2.Models
{
    public class ProformaInvoiceHdr
    {
        public int Id { get; set; }
        public string ProformaNo { get; set; }
        public DateTime? ProformaDate { get; set; }
        public string? ReverseCharge { get; set; }
        public string? State { get; set; }

        [Required(ErrorMessage = "Quotation No is required.")]
        public string? QuotationNo { get; set; }
        [Required(ErrorMessage = "Customer name is required.")]
        public string? CompanyName { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "GST No is required.")]
        public string? GstNo { get; set; }
        public decimal? AllProTotalAmount { get; set; }
        [Required(ErrorMessage = "Against By is required.")]
        public string? AgainstBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
         public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }

        public List<ProformaInvoiceDtls> InvoiceDetails { get; set; }
    }


    public class ProformaInvoiceDtls
    {
        public int Id { get; set; }
        public int ProformaId { get; set; }  
        public string ProductDescription { get; set; }
        public string SACCode { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Total { get; set; }
        public decimal? Discount { get; set; }
        public decimal? DiscountAmt { get; set; }
        public decimal? Tax { get; set; }
        public decimal? TaxAmt { get; set; }
        public decimal? GrandTotal { get; set; }

        // Navigation property to header (optional if needed for ORM)
        public ProformaInvoiceHdr ProformaInvoiceHdr { get; set; }
    }

}
