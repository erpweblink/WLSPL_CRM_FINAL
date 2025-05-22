using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WLSPL_CRM_2.Models
{
    public class RegisterUser
    {
        [Key]
        public int ID { get; set; }

        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }
        [Required(ErrorMessage = "Please Enter UserName")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please Enter Email Id")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please Enter Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string? MobileNo { get; set; }

        public string? Role { get; set; }

        public string? Department { get; set; }

        public string? PhotoPath { get; set; }

        public IFormFile? File { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsApproved { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? IPAddress { get; set; }
        public string? RelationID { get; set; }

        [NotMapped]
        public int? pendingCount { get; set; }
    }


    public class ForgetPasswordMaster
    {
        public int Id { get; set; }
        public int? userId { get; set; }
        public string? userMail { get; set; }
        public string Tempotp { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
