using System.ComponentModel.DataAnnotations;

namespace Appointment_Scheduler.Models
{
    public class UserDetails
    {
        public string UserName { get; set; }
        public string Password_U { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public string Designation { get; set; }
    }
    public class UserLoginDetails
    {
        public string Email { get; set; }
        public string Password_U { get; set; }
    }
}
