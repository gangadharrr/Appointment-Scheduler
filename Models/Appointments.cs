using System.Security.Policy;

namespace Appointment_Scheduler.Models
{
    public class Appointments
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber{ get; set; }
        public string CompanyName { get; set;}
        public string Designation { get; set;}
        public string CollaboratorEmail { get; set;}
        public string MeetingTitle { get; set;}
        public string MeetingDescription { get; set;}
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now;
        public string MeetingUrl { get; set;}
    }
}
