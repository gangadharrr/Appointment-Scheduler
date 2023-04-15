using Appointment_Scheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NuGet.Common;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Mail;


namespace Appointment_Scheduler.Controllers
{
    public class RegistrationController : Controller
    {
        IConfiguration Configuration;
        SqlConnection sqlConnection;
        SqlCommand sqlCommand;
        private static int VerificationCode=0   ;
        public RegistrationController(IConfiguration configuration)
        {
            Configuration= configuration;
            sqlConnection=new SqlConnection(Configuration.GetConnectionString("Appointment Scheduler"));
            sqlCommand = sqlConnection.CreateCommand();
        }
        // GET: RegistrationController/Login
        public ActionResult Login()
        {
            ViewData["LoginStatus"] = false;
            return View(new UserLoginDetails());
        }

        // POST: RegistrationController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLoginDetails userLoginDetails)
        {
            ViewData["LoginStatus"] = true;
            try
            {
                Console.WriteLine(userLoginDetails.Email+" "+userLoginDetails.Password_U);
                using(sqlConnection)
                {
                    sqlConnection.Open();
                    using(sqlCommand)
                    {
                        using (var md5Hash = MD5.Create())
                        {

         
                            var InputPasswordHash = BitConverter.ToString(md5Hash.ComputeHash(Encoding.UTF8.GetBytes(userLoginDetails.Password_U))).Replace("-", string.Empty);


                            Console.WriteLine("inside post sql");
                            sqlCommand.CommandText = "FETCH_PASSWORD";
                            sqlCommand.CommandType = CommandType.StoredProcedure;
                            sqlCommand.Parameters.Add(new SqlParameter("@Email", userLoginDetails.Email));
                            SqlDataReader reader = sqlCommand.ExecuteReader();
                            while (reader.Read())
                            {
                                var DatabasePasswordHash=BitConverter.ToString((byte[])reader["PASSWORD_U"]).Replace("-", string.Empty);
                                if (InputPasswordHash == DatabasePasswordHash)
                                {
                                    ViewData["LoginStatus"] = true;
                                    return RedirectToAction("Display", "Scheduler",new { Email = userLoginDetails.Email });
                                }
                            }
                        }
                    }
                }
                
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: RegistrationController/SignUp
        public ActionResult SignUp()
        {
            
            return View(new UserDetails());
        }
        // POST: RegistrationController/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(UserDetails userDetails)
        {
           
            try
            {
                using (sqlConnection)
                {
                    sqlConnection.Open();
                    using (sqlCommand)
                    {
                        sqlCommand.CommandText = "INSERT_DETAILS";
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@UserName", userDetails.UserName));
                        sqlCommand.Parameters.Add(new SqlParameter("@Password_U", userDetails.Password_U));
                        sqlCommand.Parameters.Add(new SqlParameter("@Email", userDetails.Email));
                        sqlCommand.Parameters.Add(new SqlParameter("@PhoneNumber", userDetails.PhoneNumber));
                        sqlCommand.Parameters.Add(new SqlParameter("@CompanyName", userDetails.CompanyName));
                        sqlCommand.Parameters.Add(new SqlParameter("@Designation", userDetails.Designation));
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Login));
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
                
            }
        }
     

       
        // GET: RegistrationController/Edit/5
        public ActionResult ForgotPassword()
        {

            return View();
        }

        // POST: RegistrationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword( IFormCollection collection)
        {
            try
            {
                Random rnd = new Random();
                VerificationCode = rnd.Next(100000,999999);
                SendEmail(VerificationCode, collection["Email"].ToString());
                return RedirectToAction("SecurityVerification", "Registration",new {Email= collection["Email"].ToString() });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        } 
        public async void changeCode()
        {
            Console.WriteLine("Before: "+VerificationCode);
            await Task.Delay(30000);
            VerificationCode = 0;
            Console.WriteLine("After: " + VerificationCode);
        }
        // GET: RegistrationController/Edit/5
        public ActionResult SecurityVerification(string Email)
        {
            if (VerificationCode != 0)
            {
                changeCode();
                return View();
            }
            else{
                
                return RedirectToAction(nameof(ForgotPassword));

            }
        }

        // POST: RegistrationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecurityVerification(IFormCollection collection)
        {
            try
            {
                if (Convert.ToInt32(collection["Code"]) == VerificationCode)
                {
                    VerificationCode = 0;
                    return RedirectToAction(nameof(UpdatePassword));
                }
                else
                {
                    return RedirectToAction(nameof(SecurityVerification));
                }
            }
            catch
            {
                return RedirectToAction(nameof(SecurityVerification));
            }
        }
        public ActionResult UpdatePassword()
        {

            return View();
        }

        // POST: RegistrationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(IFormCollection collection)
        {
           
                return View();
            
        }
        public static void SendEmail(int pass,string Email)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("tagbotroadster@gmail.com");
                message.To.Add(new MailAddress(Email));
                message.Subject = "Password Recovery";

                message.Body = $"Your Password Verfication code is {pass}";
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("tagbotroadster", "gpicefmynomhygmh");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception) { }
        }

    }
}
