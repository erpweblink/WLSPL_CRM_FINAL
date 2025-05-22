using System.ComponentModel.DataAnnotations;

namespace WLSPL_CRM_2.Models
{
    public class LeadGenration
    {
        [Key]
        public int ID { get; set; }


        public string Leadcode { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Company Name cannot exceed 255 characters.")]
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }

        [Required]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        public string Email { get; set; }


        [Required]
        public string Requirements { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Product cannot exceed 50 characters.")]
        public string Product { get; set; }

        public string Status { get; set; }

        [Required]
        public int Quantity { get; set; }


        [Required]
        [StringLength(50, ErrorMessage = "Source cannot exceed 50 characters.")]
        public string Source { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string City { get; set; }

        [Required]
        public string UserName { get; set; }
        public string UserID { get; set; }


        [StringLength(int.MaxValue, ErrorMessage = "Notes cannot exceed the maximum length.")]
        public string Notes { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Created By cannot exceed 50 characters.")]
        public string Createdby { get; set; }

        [StringLength(50, ErrorMessage = "Updated By cannot exceed 50 characters.")]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [StringLength(50, ErrorMessage = "Updated By cannot exceed 50 characters.")]
        public string DeletedBy { get; set; }

        public DateTime DeletedOn { get; set; }


    }
}
