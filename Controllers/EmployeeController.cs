using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TCSProject.Models;

namespace TCSProject.Controllers
{
    public class EmployeeController : Controller
    {


        SqlConnection con= new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        // GET: Login


        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        void connectionstring()
        {
            con.ConnectionString = "data source = SURYATEJA\\MS; initial catalog = EMP; user id = sa; password = monuSurya; multipleactiveresultsets = True; application name = EntityFramework";
        }


        [HttpPost]
        public ActionResult VerifyEmployee(Employee emp)
        {
            try
            {


                connectionstring();
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from EmplTable where EmpId='" + emp.EmpId + "' and Password='" + emp.Password + "'";
                dr = com.ExecuteReader();
               
                
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }


            if (dr.Read() == false)
            {
         
                emp.LoginErrorMessage = "Invalid Credentials !";
                return View("Login", emp);
            }
            else
            {
                con.Close();
                Session["EmpId"] = emp.EmpId;
                return RedirectToAction("Home","Employee");

            }
        }


    

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Employee");
        }
        

        [HttpGet]
        public ActionResult ForgotPassword()
        {

            return View();
        }


        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Employee/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("fanofmyplayer@gmail.com", "Employee Management Portal");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "monuSurya"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [HttpPost]
        public ActionResult ForgotPassword(string EmailId)
        {
            string message = "";
            bool status = false;

            connectionstring();
            con.Open();
            string insertQuery = "select count(EmpId) from EmplTable where Email='" + EmailId + "'";
            SqlCommand cmd = new SqlCommand(insertQuery, con);
            int count = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            if (count > 0)
            {
                string resetCode = Guid.NewGuid().ToString();
                SendVerificationLinkEmail(EmailId, resetCode, "ResetPassword");
                insertQuery = "Update EmplTable SET ResetPasswordCode= @RPC where Email= @mail";
                cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@RPC", resetCode);
                cmd.Parameters.AddWithValue("@mail", EmailId );
                count = cmd.ExecuteNonQuery();
                message = "Reset password link has been sent to your email id.";
            }
            else
            {
                message = "Account not found";
            }
            ViewBag.Message = message;
      
            return View();


        }



        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            connectionstring();
            con.Open();
            string insertQuery = "select count(EmpId) from EmplTable where ResetPasswordCode='" + id + "'";
            SqlCommand cmd = new SqlCommand(insertQuery, con);
            int count = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            if(count>0)
            {
                
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                
                
            }
            else
            {
                return HttpNotFound();
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "Entered Action.... ";
            
                connectionstring();
                con.Open();
                string insertQuery = "select count(EmpId) from EmplTable where ResetPasswordCode='" + model.ResetCode + "'";
                SqlCommand cmd = new SqlCommand(insertQuery, con);
                int count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                if (count > 0)
                {
                    message = "Employee exists ";

                    insertQuery = "select EmpId from EmplTable where ResetPasswordCode= '" + model.ResetCode + "'";
                    SqlCommand cmd2 = new SqlCommand(insertQuery, con);
                    string eid = cmd2.ExecuteScalar().ToString();


                    insertQuery = "Update EmplTable SET Password= @pwd , ResetPasswordCode= @rpc where EmpId= @eid";
                    SqlCommand cmd1 = new SqlCommand(insertQuery, con);
                    cmd1.Parameters.AddWithValue("@pwd", model.NewPassword);
                    cmd1.Parameters.AddWithValue("@rpc", "");
                    cmd1.Parameters.AddWithValue("@eid", eid);

                    int res = cmd1.ExecuteNonQuery();
                    if (res > 0)
                        message += "New password updated successfully";
                ViewBag.message = message;
                return View(model);
                }

            
                message = "Something invalid";
            
            ViewBag.Message = message;
            return View(model);
        }

    }
}