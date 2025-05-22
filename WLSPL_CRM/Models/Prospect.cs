namespace WLSPL_CRM_2.Models
{
    public class Prospect
    {
        public String ID { get; set; }
        public string ComapnyName { get; set; }

        public string Email { get; set; }

        public string MobileNo { get; set; }

        public string Indsegment { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Type { get; set; }

        public string Product { get; set; }

        public string Executive { get; set; }

        public string Buisnessprospect { get; set; }

        public string OrderTarget { get; set; }

        public string Stage { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string DeletedBy { get; set; }

        // Date of deletion
        public DateTime? DeletedOn { get; set; }


        public bool IsDeleted { get; set; }
        public string prospect { get; set; }
        public string customer { get; set; }
        public string supplier { get; set; }
        public string vendor { get; set; }
        public string partner { get; set; }
        public string wholesaler { get; set; }
        public string Friend { get; set; }
    }
}

