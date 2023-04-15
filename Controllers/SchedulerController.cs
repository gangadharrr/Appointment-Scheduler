using Appointment_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Data;

namespace Appointment_Scheduler.Controllers
{
    public class SchedulerController : Controller
    {
        IConfiguration Configuration;
        SqlConnection sqlConnection;
        SqlCommand sqlCommand;
        public static UserDetails userDetails=new UserDetails();
        
        public SchedulerController(IConfiguration configuration)
        {
            Configuration = configuration;
            sqlConnection = new SqlConnection(Configuration.GetConnectionString("Appointment Scheduler"));
            sqlCommand = sqlConnection.CreateCommand();
            
        }
        // GET: SchedulerController
        public ActionResult Display(string Email)
        {
            List<Appointments> appointmentList = new List<Appointments>();
            using (sqlConnection)
            {
                sqlConnection.Open();
                using (sqlCommand)
                {
                    sqlCommand.CommandText = "FETCH_DETAILS";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@EMAIL", Email));
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        userDetails.UserName = Convert.ToString(reader["UserName"]);
                        userDetails.Email = Convert.ToString(reader["Email"]);
                        userDetails.PhoneNumber = Convert.ToString(reader["PhoneNumber"]);
                        userDetails.CompanyName = Convert.ToString(reader["CompanyName"]);
                        userDetails.Designation = Convert.ToString(reader["Designation"]);
                    }
                    reader.Close();
                }
                using (sqlCommand)
                {
                    sqlCommand.CommandText = "FETCH_APPOINTMENT_DETAILS";
                    sqlCommand.CommandType = sqlCommand.CommandType;
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Parameters.Add(new SqlParameter("@EMAIL", userDetails.Email));
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        Appointments appointment = new Appointments();
                        appointment.Id = Convert.ToInt32(reader["Id"]);
                        appointment.UserName = Convert.ToString(reader["UserName"]);
                        appointment.Email = Convert.ToString(reader["Email"]);
                        appointment.PhoneNumber = Convert.ToString(reader["PhoneNumber"]);
                        appointment.CompanyName = Convert.ToString(reader["CompanyName"]);
                        appointment.Designation = Convert.ToString(reader["Designation"]);
                        appointment.CollaboratorEmail = Convert.ToString(reader["CollaboratorEmail"]);
                        appointment.MeetingTitle = Convert.ToString(reader["MeetingTitle"]);
                        appointment.MeetingDescription = Convert.ToString(reader["MeetingDescription"]);
                        appointment.StartTime = Convert.ToDateTime(reader["StartTime"]);
                        appointment.EndTime = Convert.ToDateTime(reader["EndTime"]);
                        appointment.MeetingUrl = Convert.ToString(reader["MeetingUrl"]);
                        appointmentList.Add(appointment);
                    }
                }
            }
            
            return View(appointmentList);
        }

        // GET: SchedulerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        public List<UserDetails> FetchAllUserDetails()
        {
            Console.WriteLine(Configuration.GetConnectionString("Appointment Scheduler"));
            List<UserDetails> userDetailsList = new List<UserDetails>();
            using (SqlConnection sqlConnection = new SqlConnection(Configuration.GetConnectionString("Appointment Scheduler")))
            {
                sqlConnection.Open();
                using (sqlCommand= sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "FETCH_DETAILS_ALL";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        UserDetails _userDetails = new UserDetails();
                        _userDetails.UserName = Convert.ToString(reader["UserName"]);
                        _userDetails.Email = Convert.ToString(reader["Email"]);
                        _userDetails.PhoneNumber = Convert.ToString(reader["PhoneNumber"]);
                        _userDetails.CompanyName = Convert.ToString(reader["CompanyName"]);
                        _userDetails.Designation = Convert.ToString(reader["Designation"]);
                        userDetailsList.Add(_userDetails);
                    }

                }
            }
            return userDetailsList;
        }
        // GET: SchedulerController/Create
        public ActionResult Create()
        {
            return View(FetchAllUserDetails());
        }

        // POST: SchedulerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            Console.WriteLine( collection["MeetingDescription"]);
            collection.Keys.ToList().ForEach(x => Console.WriteLine(x));
            try
            {
                using (sqlConnection)
                {
                    sqlConnection.Open();
                    using (sqlCommand)
                    {
                        Console.WriteLine("Entered Query");

                        sqlCommand.CommandText = "INSERT_APPOINTMENT_DETAILS";
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@Email", collection["Email"].ToString()));
                        sqlCommand.Parameters.Add(new SqlParameter("@CollaboratorEmail", userDetails.Email));
                        sqlCommand.Parameters.Add(new SqlParameter("@MeetingTitle", collection["MeetingTitle"].ToString()));
                        sqlCommand.Parameters.Add(new SqlParameter("@MeetingDescription", collection["MeetingDescription"].ToString()));
                        sqlCommand.Parameters.Add(new SqlParameter("@StartTime", collection["MeetingDate"]).ToString()+" " + collection["MeetingStartTime"].ToString());
                        sqlCommand.Parameters.Add(new SqlParameter("@EndTime", collection["MeetingDate"]).ToString() + " " + collection["MeetingEndTime"].ToString());
                        sqlCommand.Parameters.Add(new SqlParameter("@MeetingUrl", collection["MeetingUrl"].ToString()));
                        sqlCommand.ExecuteNonQuery();
                        Console.WriteLine("Finished Query");
                    }
                }
                return RedirectToAction("Display", "Scheduler", new { Email = userDetails.Email }); 
            }
            catch(SqlException ex)
            {
                Console.WriteLine("SqlException");
                return View(FetchAllUserDetails());
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception"+ex.Message);
                return View(FetchAllUserDetails());
            }
        }

        // GET: SchedulerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SchedulerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SchedulerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SchedulerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
