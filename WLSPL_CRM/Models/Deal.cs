namespace WLSPL_CRM_2.Models
{
    public class Deal
    {
        
            public int Id { get; set; }
            public string Company { get; set; }
            public string DealOwner { get; set; }
            public string Stage { get; set; }
            public int Probability { get; set; }
            public string ContactFrom { get; set; }
            public string Email { get; set; }
            public decimal ExpectedRevenue { get; set; }
            public decimal Amount { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
        }
    }


