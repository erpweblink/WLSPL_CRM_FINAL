using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace WLSPL_CRM_2.Models
{
    public class Invoice
    {



        public string? LeadCode { get; set; }

        public List<QuotationItem>? Items { get; set; }
        public String  ? ID { get; set; }
        public String? HeaderID { get; set; }
        public string ? Companyname { get; set; }

        public string? Quotationno { get; set; }

        public DateTime? Quotationdate { get; set; }

        public DateTime? Validdate { get; set; }

        public string? Customername { get; set; }

        public string? Mobileno { get; set; }

        public string? Emailid { get; set; }

        public string? Gstno { get; set; }
        public string? Billingaddress { get; set; }

        public string? Shippingaddress { get; set; }

        public string? CGST_Amt { get; set; }

        public string? SGST_Amt { get; set; }

        public string? IGST_Amt { get; set; }

        public string? Total_Price { get; set; }

        public string? Totalinword { get; set; }

        public string? BankDetails { get; set; }
        public bool? IsDeleted { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string? TermsandCondition { get; set; }

   
        public Quatation? ItemDetails { get; set; }
        public int? ItemCount { get; set; }

        public List<Quatation>? Quotations { get; set; }

        public string? DepartmentName { get; set; }

        public decimal? Price { get; set; }
        public string? ServicesDesc { get; set; }

        public class QuotationItem
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

        public class ITEM
        {
            public List<QuotationItem>? Items { get; set; }
        }


        //new changes
        public class QuotationDto
        {
            public String? ID { get; set; }
            public string? Quotationno { get; set; }
            public DateTime? Quotationdate { get; set; }
            public DateTime? Validdate { get; set; }
            public string? Customername { get; set; }
            public string? Companyname { get; set; }
            public string? Billingaddress { get; set; }
            public string? Shippingaddress { get; set; }
            public string? Total_Price { get; set; }


            public string? Mobileno { get; set; }

            public string? Emailid { get; set; }

            public string? Gstno { get; set; }
        }



    }

}
