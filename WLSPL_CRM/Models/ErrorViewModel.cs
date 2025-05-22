namespace WLSPL_CRM.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class UserViewModel
    {
        public int ID { get; set; }
        public string FullName => $"{FirstName} {MiddleName ?? ""} {LastName}".Trim();
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public int? RelationID { get; set; }

        public List<UserViewModel> Children { get; set; } = new List<UserViewModel>();

    }
}
