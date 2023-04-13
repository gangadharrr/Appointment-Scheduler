using Appointment_Scheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NuGet.Common;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace Appointment_Scheduler.Controllers
{
    public class RegistrationController : Controller
    {
        IConfiguration Configuration;
        SqlConnection sqlConnection;
        SqlCommand sqlCommand;
        public RegistrationController(IConfiguration configuration)
        {
            Configuration= configuration;
            sqlConnection=new SqlConnection(Configuration.GetConnectionString("Appointment Scheduler"));
            sqlCommand = sqlConnection.CreateCommand();
        }
        // GET: RegistrationController/Login
        public ActionResult Login()
        {
            return View(new UserLoginDetails());
        }

        // POST: RegistrationController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLoginDetails userLoginDetails)
        {
            
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
                            
                            var hashBytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(userLoginDetails.Password_U));

         
                            var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);


                            Console.WriteLine("inside post sql");
                            sqlCommand.CommandText = "FETCH_PASSWORD";
                            sqlCommand.CommandType = CommandType.StoredProcedure;
                            sqlCommand.Parameters.Add(new SqlParameter("@Email", userLoginDetails.Email));
                            SqlDataReader reader = sqlCommand.ExecuteReader();
                            while (reader.Read())
                            {
                                var res=BitConverter.ToString((byte[])reader["PASSWORD_U"]).Replace("-", string.Empty);
                                Console.WriteLine(hash == res);
                            }
                        }
                    }
                }
                return RedirectToAction(nameof(Login));
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
            Console.WriteLine("in post");
           
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
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RegistrationController/Edit/5
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

        // GET: RegistrationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RegistrationController/Delete/5
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
